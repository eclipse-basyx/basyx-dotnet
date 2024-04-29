/*******************************************************************************
* Copyright (c) 2023 Bosch Rexroth AG
* Author: Constantin Ziesche (constantin.ziesche@bosch.com)
*
* This program and the accompanying materials are made available under the
* terms of the MIT License which is available at
* https://github.com/eclipse-basyx/basyx-dotnet/blob/main/LICENSE
*
* SPDX-License-Identifier: MIT
*******************************************************************************/

using BaSyx.Models.AdminShell;
using BaSyx.Utils.ResultHandling;
using NLog;
using System;
using System.Threading.Tasks;

namespace DevelopmentSubmodel
{
	public static class DevelopmentSubmodel
	{
		private static readonly ILogger logger = LogManager.GetCurrentClassLogger();

		public static AssetAdministrationShell GetAssetAdministrationShell()
		{
			AssetAdministrationShell aas = new AssetAdministrationShell("SimpleAAS", new BaSyxShellIdentifier("SimpleAAS", "1.0.0"))
			{
				Description = new LangStringSet()
				{
				   new LangString("de-DE", "Einfache VWS"),
				   new LangString("en-US", "Simple AAS")
				},
				Administration = new AdministrativeInformation()
				{
					Version = "1.0",
					Revision = "120"
				},
				AssetInformation = new AssetInformation()
				{
					AssetKind = AssetKind.Instance,
					GlobalAssetId = new BaSyxAssetIdentifier("SimpleAsset", "1.0.0")
				}
			};

			Submodel testSubmodel = GetDevSubmodel();

			aas.Submodels.Add(testSubmodel);

			return aas;
		}

		public static Submodel GetDevSubmodel()
		{
			var blob = new Blob("Blob_L1");
			blob.SetValue("decaf");

			var subBlob = new Blob("Blob_L2");
			subBlob.SetValue("decaf");

			var multiLang = new MultiLanguageProperty("MultiLanguageProperty")
			{
				Description =
					new LangStringSet() { new LangString("en", "This is an exemplary MultiLanguageProperty") },
				SemanticId = new Reference(new Key(KeyType.GlobalReference,
					new BaSyxPropertyIdentifier("MultiLanguageProperty", "1.0.0").ToUrn())),
				Value = new LangStringSet()
				{
					new LangString("en", "This is a label in English"),
					new LangString("de", "Das ist ein Bezeichner in deutsch")
				}
			};

			var property = new Property<string>("Property_String", "Level 1 String")
			{
				Description = new LangStringSet() { new LangString("en", "This is an exemplary String property") },
				SemanticId = new Reference(new Key(KeyType.GlobalReference,
					new BaSyxPropertyIdentifier("Property_String", "1.0.0").ToUrn())),
			};

			var range = new BaSyx.Models.AdminShell.Range("Range")
			{
				Description = new LangStringSet() { new LangString("en", "This is an exemplary Range") },
				SemanticId = new Reference(new Key(KeyType.GlobalReference,
					new BaSyxPropertyIdentifier("Range", "1.0.0").ToUrn())),
				ValueType = new DataType(DataObjectType.Int32),
				Value = new RangeValue()
				{
					Min = new ElementValue<int>(3),
					Max = new ElementValue<int>(5)
				}
			};

			var relationship = new RelationshipElement("RelationshipElement")
			{
				Description = new LangStringSet() { new LangString("en", "This is an exemplary RelationshipElement") },
				SemanticId = new Reference(new Key(KeyType.GlobalReference,
					new BaSyxPropertyIdentifier("RelationshipElement", "1.0.0").ToUrn())),
				First = new Reference(
					new Key(KeyType.Submodel, new BaSyxSubmodelIdentifier("HelloSubmodel", "1.0.0").ToUrn()),
					new Key(KeyType.Property, new BaSyxPropertyIdentifier("HelloProperty", "1.0.0").ToUrn())),
				Second = new Reference(
					new Key(KeyType.Submodel, new BaSyxSubmodelIdentifier("HelloSubmodel", "1.0.0").ToUrn()),
					new Key(KeyType.Property, new BaSyxPropertyIdentifier("HelloPropertyInternal", "1.0.0").ToUrn())),
			};

			var annoRelationship = new AnnotatedRelationshipElement("AnnotatedRelationshipElement")
			{
				Description = new LangStringSet() { new LangString("en", "This is an exemplary RelationshipElement") },
				SemanticId = new Reference(new Key(KeyType.GlobalReference,
					new BaSyxPropertyIdentifier("RelationshipElement", "1.0.0").ToUrn())),
				First = new Reference(
					new Key(KeyType.Submodel, new BaSyxSubmodelIdentifier("HelloSubmodel", "1.0.0").ToUrn()),
					new Key(KeyType.Property, new BaSyxPropertyIdentifier("HelloProperty", "1.0.0").ToUrn())),
				Second = new Reference(
					new Key(KeyType.Submodel, new BaSyxSubmodelIdentifier("HelloSubmodel", "1.0.0").ToUrn()),
					new Key(KeyType.Property, new BaSyxPropertyIdentifier("HelloPropertyInternal", "1.0.0").ToUrn())),
				Annotations = new ElementContainer<ISubmodelElement>
				{
					new Property<string>("Anno_Property_String", "String in AnnotatedRelationshipElement")
				}
			};

			var operation = new Operation("Operation")
			{
				Description = new LangStringSet()
				{
					new LangString("en",
						"This is an exemplary operation returning the input argument with 'Hello' as prefix")
				},
				InputVariables = new OperationVariableSet() { new Property<string>("Text") },
				OutputVariables = new OperationVariableSet() { new Property<string>("ReturnValue") }
			};

			var file = new FileElement("File")
			{
				Description = new LangStringSet()
					{ new LangString("en", "This is an exemplary file attached to the Asset Administration Shell") },
				ContentType = "application/pdf",
				Value = "/HelloAssetAdministrationShell.pdf"
			};

			var refElement = new ReferenceElement("ReferenceElement")
			{
				Description = new LangStringSet() { new LangString("en", "This is an exemplary ReferenceElement") },
				SemanticId = new Reference(new Key(KeyType.GlobalReference,
					new BaSyxPropertyIdentifier("HelloReferenceElement", "1.0.0").ToUrn())),
				Value = new Reference(
					new Key(KeyType.Submodel, new BaSyxSubmodelIdentifier("HelloSubmodel", "1.0.0").ToUrn()),
					new Key(KeyType.Property, new BaSyxPropertyIdentifier("HelloProperty", "1.0.0").ToUrn()))
			};

			var basicEvent = new BasicEventElement("HelloBasicEventElement")
			{
				Description = new LangStringSet() { new LangString("en", "This is an exemplary BasicEventElement") },
				SemanticId = new Reference(new Key(KeyType.GlobalReference,
					new BaSyxPropertyIdentifier("HelloBasicEventElement", "1.0.0").ToUrn())),
				Observed = new Reference(
					new Key(KeyType.Submodel, new BaSyxSubmodelIdentifier("HelloSubmodel", "1.0.0").ToUrn()),
					new Key(KeyType.Property, new BaSyxPropertyIdentifier("HelloProperty", "1.0.0").ToUrn())),
				Direction = EventDirection.Output,
				State = EventState.On,
				MessageTopic = "boschrexroth/helloBasicEventElement",
				LastUpdate = DateTime.UtcNow.ToString(),
				MinInterval = "PT3S"
			};

			Submodel testSubmodel = new Submodel("DevSubmodel", new BaSyxSubmodelIdentifier("DevSubmodel", "1.0.0"))
			{
				Description = new LangStringSet()
				{
					new("de-DE", "Submodel für die Entwicklung"),
					new("en-US", "submodel for development")
				},
				Administration = new AdministrativeInformation()
				{
					Version = "1.0",
					Revision = "1"
				},
				DisplayName = new LangStringSet()
				{
					new("de-DE", "Submodel"),
					new("en-US", "submodel")
				},
				SubmodelElements = new ElementContainer<ISubmodelElement>
				{
					new Property<string>("Property_String_1", "Level 1 String"),
					//new Property<string>("Property_String_2", "Level 2 String"),
					//new Property<string>("Property_String_3", "Level 3 String"),
					//new Property<string>("Property_String_4", "Level 4 String"),
					//new Property<string>("Property_String_5", "Level 5 String"),
					//blob,
					//property,
					//multiLang, 
					//range,
					//relationship,
					operation,
					//file,
					//refElement,
					//basicEvent,
					//annoRelationship,
					//new SubmodelElementList("ElementList_L1")
					//{
					//	Description = new LangStringSet() { new LangString("en", "This is an exemplary SubmodelElementList") },
					//	SemanticId = new Reference(new Key(KeyType.GlobalReference, new BaSyxPropertyIdentifier("HelloSubmodelElementList", "1.0.0").ToUrn())),
					//	Value =
					//	{
					//		new Property<int>("Int_L2")
					//		{
					//			Get = (prop) =>
					//			{
					//				var random = new Random();
					//				var val = random.Next(1, 500);
					//				return Task.FromResult(val);
					//			},
					//			Set = (prop, val) =>
					//			{
					//				var myVal = val;
					//				return Task.CompletedTask;
					//			}
					//		},
					//		subBlob,
					//	}
					//},
					new SubmodelElementCollection("Collection_L1")
					{
						Value =
						{
							new Property<string>("Property_String_Level_2", "Level 2 String"),
							//subBlob,
							//new SubmodelElementCollection("Collection_L2")
							//{
							//	Value =
							//	{
							//		new Property<string>("String_L3", "Level 3 String"),
							//		subSubBlob,
							//	}
							//}
						}
					}
				}
			};

			return testSubmodel;
		}
	}
}