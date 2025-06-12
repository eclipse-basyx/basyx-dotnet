/*******************************************************************************
* Copyright (c) 2024 Bosch Rexroth AG
* Author: Constantin Ziesche (constantin.ziesche@bosch.com)
*
* This program and the accompanying materials are made available under the
* terms of the MIT License which is available at
* https://github.com/eclipse-basyx/basyx-dotnet/blob/main/LICENSE
*
* SPDX-License-Identifier: MIT
*******************************************************************************/
using BaSyx.Models.Connectivity;
using BaSyx.Models.AdminShell;
using BaSyx.Utils.ResultHandling;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using BaSyx.Models.Extensions;
using System.Text.Json;

namespace BaSyx.Utils.DependencyInjection
{
    public static class DefaultImplementation
    {
        private static IServiceCollection ServiceCollection { get; set; }
        private static IServiceProvider ServiceProvider { get; set; }
        private static JsonSerializerOptions DefaultJsonSerializerOptions { get; set; }
        private static JsonSerializerOptions FullJsonSerializerOptions { get; set; }
        private static JsonSerializerOptions MetaDataJsonSerializerOptions { get; set; }

        public static IServiceCollection AddStandardImplementation(this IServiceCollection services)
        {
            services.AddTransient<IAssetInformation, AssetInformation>();
            services.AddTransient<IAssetAdministrationShell, AssetAdministrationShell>();
            services.AddTransient<ISubmodel, Submodel>();
            services.AddTransient<IConceptDictionary, ConceptDictionary>();

            services.AddTransient<IEndpoint, Endpoint>();
            services.AddTransient<IAssetAdministrationShellRepositoryDescriptor, AssetAdministrationShellRepositoryDescriptor>();
            services.AddTransient<IAssetAdministrationShellDescriptor, AssetAdministrationShellDescriptor>();
            services.AddTransient<ISubmodelRepositoryDescriptor, SubmodelRepositoryDescriptor>();
            services.AddTransient<ISubmodelDescriptor, SubmodelDescriptor>();

            services.AddTransient(typeof(IEnumerable<IReference>), typeof(List<IReference>));
            services.AddTransient(typeof(IEnumerable<IKey>), typeof(List<IKey>));
            services.AddTransient(typeof(IEnumerable<IQualifier>), typeof(List<IQualifier>));
            services.AddTransient(typeof(IEnumerable<IEmbeddedDataSpecification>), typeof(List<IEmbeddedDataSpecification>));
            services.AddTransient(typeof(IEnumerable<IReference<ISubmodel>>), typeof(List<IReference<ISubmodel>>));
            services.AddTransient(typeof(IEnumerable<SpecificAssetId>), typeof(List<SpecificAssetId>));
            services.AddTransient(typeof(IEnumerable<IEndpoint>), typeof(List<IEndpoint>));
            services.AddTransient(typeof(IEnumerable<SecurityAttribute>), typeof(List<SecurityAttribute>));
            services.AddTransient(typeof(IQualifier), typeof(Qualifier));

            services.AddTransient(typeof(IElementContainer<>), typeof(ElementContainer<>));
            services.AddTransient(typeof(IEnumerable<IAssetAdministrationShellRepositoryDescriptor>), typeof(List<IAssetAdministrationShellRepositoryDescriptor>));
            services.AddTransient(typeof(IEnumerable<IAssetAdministrationShellDescriptor>), typeof(List<IAssetAdministrationShellDescriptor>));
            services.AddTransient(typeof(IEnumerable<ISubmodelRepositoryDescriptor>), typeof(List<ISubmodelRepositoryDescriptor>));
            services.AddTransient(typeof(IEnumerable<ISubmodelDescriptor>), typeof(List<ISubmodelDescriptor>));
            services.AddTransient(typeof(IElementContainer<IAssetAdministrationShell>), typeof(ElementContainer<IAssetAdministrationShell>));
            services.AddTransient(typeof(IElementContainer<ISubmodel>), typeof(ElementContainer<ISubmodel>));
            //services.AddTransient(typeof(IElementContainer<ISubmodelElement>), typeof(ElementContainer<ISubmodelElement>));
            services.AddTransient(typeof(IElementContainer<IConceptDictionary>), typeof(ElementContainer<IConceptDictionary>));

            services.AddTransient<IProperty, Property>();
            //services.AddTransient(typeof(IProperty<>), typeof(Property<>));
            services.AddTransient<IOperation, Operation>();
            services.AddTransient<IOperationVariableSet, OperationVariableSet>();
            services.AddTransient<IOperationVariable, OperationVariable>();
            services.AddTransient<IEventElement, BasicEventElement>();
            services.AddTransient<IBasicEventElement, BasicEventElement>();
            services.AddTransient<IEventPayload, EventPayload>();
            services.AddTransient<ISubmodelElementCollection, SubmodelElementCollection>();
            services.AddTransient<IMultiLanguageProperty, MultiLanguageProperty>();
            services.AddTransient<IRelationshipElement, RelationshipElement>();
            services.AddTransient<IAnnotatedRelationshipElement, AnnotatedRelationshipElement>();
            services.AddTransient<IReferenceElement, ReferenceElement>();
            services.AddTransient<IFileElement, FileElement>();
            services.AddTransient<IBlob, Blob>();

            services.AddTransient<IConceptDictionary, ConceptDictionary>();
            services.AddTransient<IConceptDescription, ConceptDescription>();

            services.AddTransient<IValue, ElementValue>();
            services.AddTransient<IKey, Key>();

            services.AddTransient<IResult, Result>();
            services.AddTransient(typeof(IResult<>), typeof(Result<>));
            services.AddTransient<IMessage, Message>();

            services.AddTransient<IReference, Reference>();
            services.AddTransient(typeof(IReference<IAssetAdministrationShell>), typeof(Reference<IAssetAdministrationShell>));
            services.AddTransient(typeof(IReference<ISubmodel>), typeof(Reference<ISubmodel>));
            services.AddTransient(typeof(IReference<IConceptDescription>), typeof(Reference<IConceptDescription>));

            return services;
        }

        public static IServiceCollection GetStandardServiceCollection()
        {
            if (ServiceCollection == null)
            {
                ServiceCollection = new ServiceCollection();
                ServiceCollection.AddStandardImplementation();
            }
            return ServiceCollection;
        }

        public static IServiceProvider GetStandardServiceProvider()
        {
            if (ServiceProvider == null)
            {
                IServiceCollection standardServiceCollection = GetStandardServiceCollection();
                DefaultServiceProviderFactory serviceProviderFactory = new DefaultServiceProviderFactory();
                ServiceProvider = serviceProviderFactory.CreateServiceProvider(standardServiceCollection);
            }
            return ServiceProvider;
        }

        /// <summary>
        /// Returns a JsonSerializerOptions with the full submodel element converters and options set for the full AAS model
        /// </summary>
        /// <returns></returns>
        public static JsonSerializerOptions GetFullJsonSerializerOptions()
        {
            if(FullJsonSerializerOptions == null)
            {
                DefaultJsonSerializerOptions options = new DefaultJsonSerializerOptions();
                var services = GetStandardServiceCollection();
                var extensions = new DependencyInjectionExtension(services);
                options.AddDependencyInjection(extensions);
                options.AddFullSubmodelElementConverter();
                FullJsonSerializerOptions = options.Build();
            }
            return FullJsonSerializerOptions;
        }

        /// <summary>
        /// Returns a JsonSerializerOptions with the metadata submodel element converters and options set for the full AAS model
        /// </summary>
        /// <returns></returns>
        public static JsonSerializerOptions GetMetaDataJsonSerializerOptions()
        {
            if (MetaDataJsonSerializerOptions == null)
            {
                DefaultJsonSerializerOptions options = new DefaultJsonSerializerOptions();
                var services = GetStandardServiceCollection();
                var extensions = new DependencyInjectionExtension(services);
                options.AddDependencyInjection(extensions);
                options.AddMetadataSubmodelElementConverter();
                MetaDataJsonSerializerOptions = options.Build();
            }
            return MetaDataJsonSerializerOptions;
        }

        /// <summary>
        /// Returns the default JsonSerializerOptions
        /// </summary>
        /// <returns></returns>
        public static JsonSerializerOptions GetDefaultJsonSerializerOptions()
        {
            if (DefaultJsonSerializerOptions == null)
            {
                DefaultJsonSerializerOptions options = new DefaultJsonSerializerOptions();
                var services = GetStandardServiceCollection();
                var extensions = new DependencyInjectionExtension(services);
                options.AddDependencyInjection(extensions);
                DefaultJsonSerializerOptions = options.Build();
            }
            return DefaultJsonSerializerOptions;
        }
    }
}
