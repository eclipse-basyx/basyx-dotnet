/*******************************************************************************
* Copyright (c) 2020 Robert Bosch GmbH
* Author: Constantin Ziesche (constantin.ziesche@bosch.com)
*
* This program and the accompanying materials are made available under the
* terms of the Eclipse Public License 2.0 which is available at
* http://www.eclipse.org/legal/epl-2.0
*
* SPDX-License-Identifier: EPL-2.0
*******************************************************************************/
using BaSyx.Models.Connectivity.Descriptors;
using BaSyx.Models.Core.AssetAdministrationShell.Generics;
using BaSyx.Models.Core.AssetAdministrationShell.Implementations;
using BaSyx.Models.Core.AssetAdministrationShell.Identification;
using BaSyx.Models.Core.AssetAdministrationShell.Semantics;
using BaSyx.Models.Core.AssetAdministrationShell.Views;
using BaSyx.Models.Core.Common;
using BaSyx.Utils.ResultHandling;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace BaSyx.Utils.DependencyInjection
{
    public static class DefaultImplementation
    {
        public static IServiceCollection AddStandardImplementation(this IServiceCollection services)
        {
            services.AddTransient<IAsset, Asset>();
            services.AddTransient<IAssetAdministrationShell, AssetAdministrationShell>();
            services.AddTransient<ISubmodel, Submodel>();
            services.AddTransient<IView, View>();

            services.AddTransient<IAssetAdministrationShellRepositoryDescriptor, AssetAdministrationShellRepositoryDescriptor>();
            services.AddTransient<IAssetAdministrationShellDescriptor, AssetAdministrationShellDescriptor>();
            services.AddTransient<ISubmodelDescriptor, SubmodelDescriptor>();

            services.AddTransient(typeof(IElementContainer<>), typeof(ElementContainer<>));
            services.AddTransient(typeof(IElementContainer<IAssetAdministrationShellDescriptor>), typeof(ElementContainer<AssetAdministrationShellDescriptor>));
            services.AddTransient(typeof(IElementContainer<ISubmodelDescriptor>), typeof(ElementContainer<SubmodelDescriptor>));            
            services.AddTransient(typeof(IQueryableElementContainer<>), typeof(QueryableElementContainer<>));
            services.AddTransient(typeof(IQueryableElementContainer<IAssetAdministrationShellDescriptor>), typeof(QueryableElementContainer<AssetAdministrationShellDescriptor>));
            services.AddTransient(typeof(IQueryableElementContainer<ISubmodelDescriptor>), typeof(QueryableElementContainer<SubmodelDescriptor>));

            services.AddTransient<IOperationVariableSet, OperationVariableSet>();
            services.AddTransient<IOperationVariable, OperationVariable>();

            services.AddTransient<IProperty, Property>();
            services.AddTransient(typeof(IProperty<>), typeof(Property<>));
            services.AddTransient<IOperation, Operation>();
            services.AddTransient<IEvent, Event>();
            services.AddTransient<ISubmodelElementCollection, SubmodelElementCollection>();
            services.AddTransient<IMultiLanguageProperty, MultiLanguageProperty>();
            services.AddTransient<IRelationshipElement, RelationshipElement>();
            services.AddTransient<IReferenceElement, ReferenceElement>();
            services.AddTransient<IFile, File>();
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
            services.AddTransient(typeof(IReference<IAsset>), typeof(Reference<Asset>));
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
