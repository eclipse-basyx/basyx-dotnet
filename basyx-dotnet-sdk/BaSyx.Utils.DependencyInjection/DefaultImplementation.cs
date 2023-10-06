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
using BaSyx.Models.Connectivity;
using BaSyx.Models.AdminShell;
using BaSyx.Utils.ResultHandling;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace BaSyx.Utils.DependencyInjection
{
    public static class DefaultImplementation
    {
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

            services.AddTransient(typeof(IElementContainer<>), typeof(ElementContainer<>));
            services.AddTransient(typeof(IQueryableElementContainer<>), typeof(QueryableElementContainer<>));
            //services.AddTransient(typeof(IEnumerable<IAssetAdministrationShellRepositoryDescriptor>), typeof(List<IAssetAdministrationShellRepositoryDescriptor>));
            //services.AddTransient(typeof(IEnumerable<IAssetAdministrationShellDescriptor>), typeof(List<IAssetAdministrationShellDescriptor>));
            //services.AddTransient(typeof(IEnumerable<ISubmodelRepositoryDescriptor>), typeof(List<ISubmodelRepositoryDescriptor>));
            //services.AddTransient(typeof(IEnumerable<ISubmodelDescriptor>), typeof(List<ISubmodelDescriptor>));
            services.AddTransient(typeof(IElementContainer<IAssetAdministrationShell>), typeof(ElementContainer<IAssetAdministrationShell>));
            services.AddTransient(typeof(IElementContainer<ISubmodel>), typeof(ElementContainer<ISubmodel>));
            services.AddTransient(typeof(IElementContainer<ISubmodelElement>), typeof(ElementContainer<ISubmodelElement>));
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
            services.AddTransient(typeof(IReference<IAssetAdministrationShell>), typeof(Reference<AssetAdministrationShell>));
            services.AddTransient(typeof(IReference<ISubmodel>), typeof(Reference<Submodel>));
            services.AddTransient(typeof(IReference<IConceptDescription>), typeof(Reference<ConceptDescription>));

            return services;
        }

        public static IServiceCollection GetStandardServiceCollection()
        {
            IServiceCollection services = new ServiceCollection();
            services.AddStandardImplementation();
            return services;
        }

        public static IServiceProvider GetStandardServiceProvider()
        {
            IServiceCollection standardServiceCollection = GetStandardServiceCollection();
            DefaultServiceProviderFactory serviceProviderFactory = new DefaultServiceProviderFactory();
            return serviceProviderFactory.CreateServiceProvider(standardServiceCollection);
        }
    }
}
