/*******************************************************************************
* Copyright (c) 2022 Bosch Rexroth AG
* Author: Constantin Ziesche (constantin.ziesche@bosch.com)
*
* This program and the accompanying materials are made available under the
* terms of the MIT License which is available at
* https://github.com/eclipse-basyx/basyx-dotnet/blob/main/LICENSE
*
* SPDX-License-Identifier: MIT
*******************************************************************************/
using BaSyx.Components.Common.Abstractions;
using BaSyx.Models.Extensions;
using BaSyx.Utils.Assembly;
using BaSyx.Utils.DependencyInjection;
using BaSyx.Utils.ResultHandling;
using BaSyx.Utils.Settings;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace BaSyx.Components.Common
{
    public abstract class ServerApplication : IServerApplication
    {
        private static readonly ILogger logger = LoggingExtentions.CreateLogger<ServerApplication>();
        
        private bool _secure = false;
        private string _defaultRoute = null;
        private string _pathBase = null;
        private JsonSerializerOptions _jsonSerializerOptions = null;

        private readonly List<Action<IApplicationBuilder>> AppBuilderPipeline;
        private readonly List<Action<IServiceCollection>> ServiceBuilderPipeline;
        private readonly List<Action<IEndpointRouteBuilder>> EndpointBuilderPipeline;

        public const string DEFAULT_CONTENT_ROOT = "Content";
        public const string DEFAULT_WEB_ROOT = "wwwroot";
        public const string UI_RELATIVE_PATH = "/ui";
        public const string FILES_PATH = "/files";
        public const string ERROR_PATH = "/error";
        public const string CONTROLLER_ASSEMBLY_NAME = "BaSyx.API.Http.Controllers";

        public Assembly ControllerAssembly { get; private set; }
        public ServerSettings Settings { get; protected set; }
        public IWebHostBuilder WebHostBuilder { get; protected set; }
        public IConfiguration Configuration { get; protected set; }

        public string ExecutionPath { get; }

        public Action ApplicationStarted { get; set; }

        public Action ApplicationStopping { get; set; }

        public Action ApplicationStopped { get; set; }

        public string ContentRoot { get; private set; }

        public string WebRoot { get; private set; }

        protected ServerApplication() : this(null, null)
        { }
        protected ServerApplication(ServerSettings settings) : this(settings, null)
        { }
        protected ServerApplication(ServerSettings settings, string[] webHostBuilderArgs)
        {
            ExecutionPath = settings?.ExecutionPath ?? Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            
            if (settings == null && !EmbeddedResource.CheckOrWriteRessourceToFile(typeof(ServerApplication).Assembly, Path.Combine(ExecutionPath, ServerSettings.FileName)))
                logger.LogError($"{ServerSettings.FileName} cannot be loaded or written to filesystem");

            Settings = settings ?? ServerSettings.LoadSettingsFromFile(ServerSettings.FileName) ?? throw new ArgumentNullException(nameof(settings));
            ControllerAssembly = Assembly.Load(CONTROLLER_ASSEMBLY_NAME);

            _jsonSerializerOptions = new JsonSerializerOptions(JsonSerializerDefaults.Web);

            webHostBuilderArgs ??= Environment.GetCommandLineArgs();
            WebHostBuilder = DefaultWebHostBuilder
                .CreateWebHostBuilder(webHostBuilderArgs, Settings)
                .ConfigureAppConfiguration((context, builder) =>
                {
                    Configuration = builder.Build();
                });
            AppBuilderPipeline = new List<Action<IApplicationBuilder>>();
            ServiceBuilderPipeline = new List<Action<IServiceCollection>>();
            EndpointBuilderPipeline = new List<Action<IEndpointRouteBuilder>>();

            WebHostBuilder.ConfigureServices( (context, services) =>
            {
                ConfigureServices(context, services);
            });

            WebHostBuilder.Configure(app =>
            {
                Configure(app);
            });

            ConfigureLogging(LogLevel.Trace);

            if (string.IsNullOrEmpty(Settings.ServerConfig.Hosting.ContentPath))
                ContentRoot = Path.Join(ExecutionPath, DEFAULT_CONTENT_ROOT);
            else if (Path.IsPathRooted(Settings.ServerConfig.Hosting.ContentPath))
                ContentRoot = Settings.ServerConfig.Hosting.ContentPath;
            else
                ContentRoot = Path.Join(ExecutionPath, Settings.ServerConfig.Hosting.ContentPath);

            try
            {
                if (!Directory.Exists(ContentRoot))
                    Directory.CreateDirectory(ContentRoot);
                WebHostBuilder.UseContentRoot(ContentRoot);
            }
            catch (Exception e)
            {
                logger.LogError(e, $"ContentRoot path {ContentRoot} cannot be created ");
            }

            WebRoot = Path.Join(ExecutionPath, DEFAULT_WEB_ROOT);
            _defaultRoute = Settings.ServerConfig.DefaultRoute;

            try
            {
                if (!Directory.Exists(WebRoot))
                    Directory.CreateDirectory(WebRoot);
                WebHostBuilder.UseWebRoot(WebRoot);
                logger.LogInformation($"wwwroot-Path: {WebRoot}");
            }
            catch (Exception e)
            {
                logger.LogError(e, $"WebRoot path {WebRoot} cannot be created ");
            }            
        }

        public virtual void Run()
        {
            logger.LogInformation("Starting Server...");

            WebHostBuilder.Build().Run();
        }
        public virtual async Task RunAsync(CancellationToken cancellationToken = default)
        {
            logger.LogInformation("Starting Server asynchronously...");

            await WebHostBuilder.Build().RunAsync(cancellationToken);
        }

        public virtual void ConfigureLogging(Action<ILoggingBuilder> logging)
        {
            WebHostBuilder.ConfigureLogging(configureLogging =>
            {
                logging.Invoke(configureLogging);
            });
        }
        public virtual void ConfigureLogging(LogLevel minimumLogLevel)
        {
            WebHostBuilder.ConfigureLogging(logging =>
            {
                logging.ClearProviders();
                logging.SetMinimumLevel(minimumLogLevel);
            });
        }
        public virtual void Configure(Action<IApplicationBuilder> app) => AppBuilderPipeline.Add(app);
        public virtual void ConfigureServices(Action<IServiceCollection> services) => ServiceBuilderPipeline.Add(services);
        public virtual void ConfigureEndpoints(Action<IEndpointRouteBuilder> endpoints) => EndpointBuilderPipeline.Add(endpoints);
        public virtual void UseContentRoot(string contentRoot)
        {
            ContentRoot = contentRoot;
            WebHostBuilder.UseContentRoot(contentRoot);
        }
        public virtual void UseWebRoot(string webRoot)
        {
            WebRoot = webRoot;
            WebHostBuilder.UseWebRoot(webRoot);
        }
        public virtual void UseUrls(params string[] urls)
        {
            WebHostBuilder.UseUrls(urls);
            if (Settings?.ServerConfig?.Hosting != null)
                Settings.ServerConfig.Hosting.Urls = urls?.ToList();
        }
        public virtual void UsePathBase(string pathBase)
        {
            _pathBase = pathBase;
        }
        public virtual void ProvideContent(Uri relativeUri, Stream content)
        {
            try
            {
                using (Stream stream = content)
                {
                    string fileName = Path.GetFileName(relativeUri.ToString());
                    logger.LogInformation("FileName: " + fileName);
                    string directory = Path.GetDirectoryName(relativeUri.ToString()).TrimStart('\\');
                    logger.LogInformation("Directory: " + directory);

                    string hostingDirectory = Path.Join(ContentRoot, directory);

                    logger.LogInformation($"Try creating hosting directory if not existing: {hostingDirectory}");
                    Directory.CreateDirectory(hostingDirectory);

                    string filePath = Path.Join(hostingDirectory, fileName);
                    logger.LogInformation($"Try writing file: {filePath}");

                    using (FileStream fileStream = File.OpenWrite(filePath))
                    {
                        stream.CopyTo(fileStream);
                    }
                }
            }
            catch (Exception e)
            {
                logger.LogError(e, $"Error providing content {relativeUri}");
            }
        }
        public virtual void MapControllers(ControllerConfiguration controllerConfig)
        {
            this.ConfigureServices(services =>
            {
                if (controllerConfig?.Controllers?.Count > 0)
                {
                    var mvcBuilder = services.AddMvc();
                    foreach (var controllerAssemblyString in controllerConfig.Controllers)
                    {
                        Assembly controllerAssembly = null;
                        try
                        {
                            controllerAssembly = Assembly.Load(controllerAssemblyString);
                        }
                        catch (Exception e)
                        {
                            logger.LogWarning(e, $"Assembly {controllerAssemblyString} cannot be loaded - maybe it is not referenced. Try reading from file...");
                            try
                            {
                                if (File.Exists(controllerAssemblyString))
                                    controllerAssembly = Assembly.LoadFile(controllerAssemblyString);
                                else if (File.Exists(controllerAssemblyString + ".dll"))
                                    controllerAssembly = Assembly.LoadFile(controllerAssemblyString + ".dll");
                                else
                                    controllerAssembly = Assembly.LoadFrom(controllerAssemblyString);
                            }
                            catch (Exception exp)
                            {
                                logger.LogWarning(exp, $"Assembly {controllerAssemblyString} can finally not be loaded");
                            }
                        }
                        if (controllerAssembly != null)
                        {
                            mvcBuilder.AddApplicationPart(controllerAssembly);
                            string controllerAssemblyName = controllerAssembly.GetName().Name;
                            string xmlDocFile = $"{controllerAssemblyName}.xml";
                            string xmlDocFilePath = Path.Combine(ExecutionPath, xmlDocFile);

                            if (File.Exists(xmlDocFilePath))
                                continue;

                            try
                            {
                                ManifestEmbeddedFileProvider embeddedFileProvider = new ManifestEmbeddedFileProvider(controllerAssembly);
                                IFileInfo fileInfo = embeddedFileProvider.GetFileInfo(xmlDocFile);
                                if (fileInfo == null)
                                {
                                    logger.LogWarning($"{xmlDocFile} of Assembly {controllerAssemblyName} not found");
                                    continue;
                                }
                                using (Stream stream = fileInfo.CreateReadStream())
                                {
                                    using (FileStream fileStream = File.OpenWrite(xmlDocFilePath))
                                    {
                                        stream.CopyTo(fileStream);
                                    }
                                }
                                logger.LogInformation($"{xmlDocFile} of Assembly {controllerAssemblyName} has been created successfully");
                            }
                            catch (Exception e)
                            {
                                logger.LogWarning(e, $"{xmlDocFile} of Assembly {controllerAssemblyName} cannot be read");
                            }
                        }
                    }
                    mvcBuilder.AddControllersAsServices();
                }
            });
        }
        protected virtual void ConfigureServices(WebHostBuilderContext context, IServiceCollection services)
        {
            services.AddSingleton(typeof(ServerSettings), Settings);
            services.AddSingleton<IServerApplicationLifetime>(this);

            services.AddExceptionHandler(options =>
            {
                options.ExceptionHandler = ExceptionHandler;
            });

            var urls = Settings.ServerConfig.Hosting.Urls;
            var secureUrl = urls.Find(s => s.StartsWith("https"));
            if (!string.IsNullOrEmpty(secureUrl) && !context.HostingEnvironment.IsDevelopment())
            {
                secureUrl = secureUrl.Replace("+", "0.0.0.0");
                Uri secureUri = new Uri(secureUrl);
                _secure = true;
                services.AddHttpsRedirection(opts =>
                {
                    opts.HttpsPort = secureUri.Port;   
                });
            }

            services.AddStandardImplementation();

            services.AddCors();
            services
                .AddMvc(options =>
                {
                    options.RespectBrowserAcceptHeader = true;
                })
                .AddApplicationPart(ControllerAssembly)
                .AddControllersAsServices()
                .AddJsonOptions(options =>
                {
                    options.GetDefaultJsonOptions(services);
                });

            services.AddRazorPages(opts =>
            {
                logger.LogInformation("Pages-RootDirectory: " + opts.RootDirectory);
            });

            services.AddDirectoryBrowser();

            services.PostConfigure<ApiBehaviorOptions>(options =>
            {
                options.InvalidModelStateResponseFactory = actionContext =>
                {
                    ValidationProblemDetails details = actionContext.ModelState
                        .Where(e => e.Value.Errors.Count > 0)
                        .Select(e => new ValidationProblemDetails(actionContext.ModelState)).FirstOrDefault();

                    List<IMessage> messages = new List<IMessage>();
                    messages.Add(new Message(
                           MessageType.Error,
                           $"Path '{actionContext.HttpContext.Request.Path.Value}' received invalid or malformed request",
                           actionContext.HttpContext.Response.StatusCode.ToString()));

                    foreach (var error in details.Errors.Values)
                    {
                        string joinedError = string.Join(" | ", error);
                        messages.Add(new Message(MessageType.Error, joinedError));
                    }

                    Result result = new Result(false, messages);

                    return new BadRequestObjectResult(result);
                };
            });

            foreach (var serviceBuider in ServiceBuilderPipeline)
            {
                serviceBuider.Invoke(services);
            }
        }

        private async Task ExceptionHandler(HttpContext context)
        {
            var feature = context.Features.Get<IExceptionHandlerFeature>();
            var exception = feature?.Error;

            if (exception != null)
            {
                context.Response.ContentType = "application/json";
                context.Response.StatusCode = 400;

                Result result = new Result(exception);
                result.Messages.Add(new Message(MessageType.Exception, exception.StackTrace));

                string resultMessage = JsonSerializer.Serialize(result, _jsonSerializerOptions);

                await context.Response.WriteAsync(resultMessage);
            }
        }

        private async Task StatusCodeHandler(StatusCodeContext context)
        {
            context.HttpContext.Response.ContentType = "application/json";

            Result result = new Result(false,
                new Message(MessageType.Error, "Path: " + context.HttpContext.Request.Path.Value,
                context.HttpContext.Response.StatusCode.ToString()));

            string resultMessage = JsonSerializer.Serialize(result, _jsonSerializerOptions);

            await context.HttpContext.Response.WriteAsync(resultMessage);
        }

        protected virtual void Configure(IApplicationBuilder app)
        {
            var env = app.ApplicationServices.GetRequiredService<IWebHostEnvironment>();
            var applicationLifetime = app.ApplicationServices.GetRequiredService<IHostApplicationLifetime>();
            var loggingFactory = app.ApplicationServices.GetRequiredService<ILoggerFactory>();
            
            //Set ASP.NET Core LoggingFactory to global LoggingFactory for all other loggings within the BaSyx SDK
            LoggingExtentions.LoggerFactory = loggingFactory;

            if (env.IsProduction())
                app.UseHsts();

            if (!string.IsNullOrEmpty(_pathBase))
            {
                app.UsePathBase(_pathBase);
                app.Use(async (context, next) =>
                {
                    if (!context.Request.PathBase.HasValue)
                    {
                        context.Response.Redirect(_pathBase + context.Request.Path);
                        return;
                    }
                    await next();
                });
            }

            app.UseExceptionHandler();

            app.UseStatusCodePages(StatusCodeHandler);

            if(_secure && !env.IsDevelopment())
                app.UseHttpsRedirection();
            
            app.UseStaticFiles();

            string path = env.ContentRootPath;
            if (Directory.Exists(path))
            {
                app.UseStaticFiles(new StaticFileOptions()
                {
                    FileProvider = new PhysicalFileProvider(@path),
                    RequestPath = new PathString(FILES_PATH),
                    ServeUnknownFileTypes = true
                });

                app.UseDirectoryBrowser(new DirectoryBrowserOptions
                {
                    FileProvider = new PhysicalFileProvider(@path),
                    RequestPath = new PathString(FILES_PATH)
                });
            }

            //This middleware fixes the issue with reverse proxies (e.g. IIS) decoding URLs within http paths
            //app.Use((context, next) =>
            //{
            //    var url = context.GetServerVariable("UNENCODED_URL");
            //    if (!string.IsNullOrEmpty(url))
            //        context.Request.Path = new PathString(url);

            //    return next();
            //});

            foreach (var appBuilder in AppBuilderPipeline)
            {
                appBuilder.Invoke(app);
            }

            app.UseRouting();

            app.UseCors(
                options => options
                            .AllowAnyHeader()
                            .AllowAnyMethod()
                            .AllowAnyOrigin()
            );

            app.UseAuthentication();
            app.UseAuthorization();
            
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
                endpoints.MapControllers();

                foreach (var endpointBuilder in EndpointBuilderPipeline)
                {
                    endpointBuilder.Invoke(endpoints);
                }
            });

            if (!string.IsNullOrEmpty(_defaultRoute))
            {
                var options = new RewriteOptions().AddRedirect("^$", _defaultRoute);
                app.UseRewriter(options);
            }

            if (ApplicationStarted != null)
                applicationLifetime.ApplicationStarted.Register(ApplicationStarted);
            if (ApplicationStopping != null)
                applicationLifetime.ApplicationStopping.Register(ApplicationStopping);
            if (ApplicationStopped != null)
                applicationLifetime.ApplicationStopped.Register(ApplicationStopped);            
        }        
    }
}
