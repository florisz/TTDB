<?xml version="1.0" encoding="utf-8"?>
<Dsl dslVersion="1.0.0.0" Id="1ed0c2a9-2a9e-43d2-8fe6-3d731d161d11" Description="Description for Luminis.Its.Workbench.Workbench" Name="Workbench" DisplayName="Class Diagrams" Namespace="Luminis.Its.Workbench" ProductName="Its" CompanyName="Luminis" PackageGuid="c9659392-bc30-455b-ab3b-9b3f64727a62" PackageNamespace="" xmlns="http://schemas.microsoft.com/VisualStudio/2005/DslTools/DslDefinitionModel">
  <Classes>
    <DomainClass Id="0ea4c852-dd0b-4c9a-8707-76724082a360" Description="" Name="NamedElement" DisplayName="Named Element" InheritanceModifier="Abstract" Namespace="Luminis.Its.Workbench">
      <Properties>
        <DomainProperty Id="6d98f9d1-3384-423d-b718-7127da4ec470" Description="" Name="Name" DisplayName="Name" DefaultValue="" IsElementName="true">
          <Type>
            <ExternalTypeMoniker Name="/System/String" />
          </Type>
        </DomainProperty>
      </Properties>
    </DomainClass>
    <DomainClass Id="01702fe4-aa67-4799-8b5a-e8fc6e79d106" Description="" Name="ModelRoot" DisplayName="Model Root" Namespace="Luminis.Its.Workbench">
      <BaseClass>
        <DomainClassMoniker Name="NamedElement" />
      </BaseClass>
      <ElementMergeDirectives>
        <ElementMergeDirective>
          <Index>
            <DomainClassMoniker Name="Comment" />
          </Index>
          <LinkCreationPaths>
            <DomainPath>ModelRootHasComments.Comments</DomainPath>
          </LinkCreationPaths>
        </ElementMergeDirective>
        <ElementMergeDirective>
          <Index>
            <DomainClassMoniker Name="ObjectModelSpec" />
          </Index>
          <LinkCreationPaths>
            <DomainPath>HasObjectModelSpec.ObjectModelSpec</DomainPath>
          </LinkCreationPaths>
        </ElementMergeDirective>
        <ElementMergeDirective>
          <Index>
            <DomainClassMoniker Name="CaseFileModelSpec" />
          </Index>
          <LinkCreationPaths>
            <DomainPath>HasCaseFileModelSpec.CaseFileModelSpecs</DomainPath>
          </LinkCreationPaths>
        </ElementMergeDirective>
      </ElementMergeDirectives>
    </DomainClass>
    <DomainClass Id="65ad2849-6f91-4739-adba-28d37b6b2af2" Description="An attribute of a class." Name="ModelAttribute" DisplayName="Model Attribute" Namespace="Luminis.Its.Workbench">
      <BaseClass>
        <DomainClassMoniker Name="ObjectModelElement" />
      </BaseClass>
      <Properties>
        <DomainProperty Id="6e32f884-a0cc-4381-90a7-1dc656c65aee" Description="" Name="Type" DisplayName="Type" DefaultValue="">
          <Type>
            <ExternalTypeMoniker Name="/System/String" />
          </Type>
        </DomainProperty>
        <DomainProperty Id="d827311b-d9d7-412f-8170-859ce990bd1a" Description="" Name="InitialValue" DisplayName="Initial Value" DefaultValue="">
          <Type>
            <ExternalTypeMoniker Name="/System/String" />
          </Type>
        </DomainProperty>
        <DomainProperty Id="de3310e7-28d2-4e95-b7fe-c2ffd3e2f6d6" Description="Description for Luminis.Its.Workbench.ModelAttribute.Required" Name="Required" DisplayName="Required">
          <Type>
            <ExternalTypeMoniker Name="/System/Boolean" />
          </Type>
        </DomainProperty>
      </Properties>
    </DomainClass>
    <DomainClass Id="ec0ef121-6162-408f-8ab3-69aa325c49f3" Description="" Name="Comment" DisplayName="Comment" Namespace="Luminis.Its.Workbench">
      <Properties>
        <DomainProperty Id="a5798a13-c0b4-42e6-9417-9ec1240ef5cc" Description="" Name="Text" DisplayName="Text" DefaultValue="">
          <Type>
            <ExternalTypeMoniker Name="/System/String" />
          </Type>
        </DomainProperty>
      </Properties>
    </DomainClass>
    <DomainClass Id="1fd46933-711c-47ae-bde4-b098b508e583" Description="" Name="ModelType" DisplayName="Model Type" InheritanceModifier="Abstract" Namespace="Luminis.Its.Workbench">
      <BaseClass>
        <DomainClassMoniker Name="ObjectModelElement" />
      </BaseClass>
      <ElementMergeDirectives>
        <ElementMergeDirective>
          <Index>
            <DomainClassMoniker Name="Comment" />
          </Index>
          <LinkCreationPaths>
            <DomainPath>CommentReferencesSubjects.Comments</DomainPath>
            <DomainPath>ObjectModelSpecHasTypes.ObjectModelSpec/!ObjectModelSpec/HasObjectModelSpec.ModelRoot/!ModelRoot/ModelRootHasComments.Comments</DomainPath>
          </LinkCreationPaths>
        </ElementMergeDirective>
        <ElementMergeDirective>
          <Index>
            <DomainClassMoniker Name="ModelAttribute" />
          </Index>
          <LinkCreationPaths>
            <DomainPath>ModelTypeHasAttributes.Attributes</DomainPath>
          </LinkCreationPaths>
        </ElementMergeDirective>
      </ElementMergeDirectives>
    </DomainClass>
    <DomainClass Id="d54639a7-5344-4749-8ec2-a7f9c69f00c1" Description="Element with a Description" Name="ObjectModelElement" DisplayName="Object Model Element" InheritanceModifier="Abstract" Namespace="Luminis.Its.Workbench">
      <Notes>Abstract base of all elements that have a Description property.</Notes>
      <BaseClass>
        <DomainClassMoniker Name="NamedElement" />
      </BaseClass>
      <Properties>
        <DomainProperty Id="23f51d5c-62e4-4521-bc07-49039c4c94d8" Description="This is a Description." Name="Description" DisplayName="Description" DefaultValue="">
          <Type>
            <ExternalTypeMoniker Name="/System/String" />
          </Type>
        </DomainProperty>
      </Properties>
    </DomainClass>
    <DomainClass Id="eee95722-bbba-4040-b8d3-96c4b76a9d9e" Description="Description for Luminis.Its.Workbench.ObjectModelSpec" Name="ObjectModelSpec" DisplayName="Object Model Spec" Namespace="Luminis.Its.Workbench">
      <BaseClass>
        <DomainClassMoniker Name="NamedElement" />
      </BaseClass>
      <Properties>
        <DomainProperty Id="5f8af9cc-9fe2-4c36-ac5f-5da5f9e96574" Description="Description for Luminis.Its.Workbench.ObjectModelSpec.Self" Name="Self" DisplayName="Self" IsUIReadOnly="true">
          <Type>
            <ExternalTypeMoniker Name="/System/String" />
          </Type>
        </DomainProperty>
      </Properties>
      <ElementMergeDirectives>
        <ElementMergeDirective>
          <Index>
            <DomainClassMoniker Name="ModelType" />
          </Index>
          <LinkCreationPaths>
            <DomainPath>ObjectModelSpecHasTypes.Types</DomainPath>
          </LinkCreationPaths>
        </ElementMergeDirective>
      </ElementMergeDirectives>
    </DomainClass>
    <DomainClass Id="962ac54d-2708-48a1-8686-fe18b688f545" Description="Description for Luminis.Its.Workbench.CaseFileModelSpec" Name="CaseFileModelSpec" DisplayName="Case File Model Spec" Namespace="Luminis.Its.Workbench">
      <BaseClass>
        <DomainClassMoniker Name="NamedElement" />
      </BaseClass>
      <Properties>
        <DomainProperty Id="1e39d0c4-660d-43e0-8810-8528eea4729b" Description="Description for Luminis.Its.Workbench.CaseFileModelSpec.Uri Template" Name="UriTemplate" DisplayName="Uri Template">
          <Type>
            <ExternalTypeMoniker Name="/System/String" />
          </Type>
        </DomainProperty>
        <DomainProperty Id="7a979ff7-7a43-465b-afdc-1ad5d291162b" Description="Description for Luminis.Its.Workbench.CaseFileModelSpec.Self" Name="Self" DisplayName="Self" IsUIReadOnly="true">
          <Type>
            <ExternalTypeMoniker Name="/System/String" />
          </Type>
        </DomainProperty>
        <DomainProperty Id="656bf97b-bc88-44b3-aba5-b68ede9fe4b1" Description="Description for Luminis.Its.Workbench.CaseFileModelSpec.Object Model Spec" Name="ObjectModelSpec" DisplayName="Object Model Spec" IsUIReadOnly="true">
          <Type>
            <ExternalTypeMoniker Name="/System/String" />
          </Type>
        </DomainProperty>
      </Properties>
      <ElementMergeDirectives>
        <ElementMergeDirective>
          <Index>
            <DomainClassMoniker Name="CaseFileType" />
          </Index>
          <LinkCreationPaths>
            <DomainPath>CaseFileModelSpecHasCaseFileTypes.CaseFileTypes</DomainPath>
          </LinkCreationPaths>
        </ElementMergeDirective>
      </ElementMergeDirectives>
    </DomainClass>
    <DomainClass Id="8851c9ec-eb94-4286-8839-b50f3a14898d" Description="Description for Luminis.Its.Workbench.CaseFileType" Name="CaseFileType" DisplayName="Case File Type" InheritanceModifier="Abstract" Namespace="Luminis.Its.Workbench">
      <BaseClass>
        <DomainClassMoniker Name="NamedElement" />
      </BaseClass>
    </DomainClass>
    <DomainClass Id="9dc988ac-2e68-4fe4-aa6e-bac1071d34bb" Description="Description for Luminis.Its.Workbench.CaseFileEntity" Name="CaseFileEntity" DisplayName="Case File Entity" Namespace="Luminis.Its.Workbench">
      <BaseClass>
        <DomainClassMoniker Name="CaseFileType" />
      </BaseClass>
    </DomainClass>
    <DomainClass Id="9aacdca2-eca2-488e-8cbc-e543995d5f8e" Description="Description for Luminis.Its.Workbench.CaseFileRelation" Name="CaseFileRelation" DisplayName="Case File Relation" Namespace="Luminis.Its.Workbench">
      <BaseClass>
        <DomainClassMoniker Name="CaseFileType" />
      </BaseClass>
    </DomainClass>
    <DomainClass Id="75b90a61-e133-4c4f-b361-e394f1a5df81" Description="Description for Luminis.Its.Workbench.ModelRelation" Name="ModelRelation" DisplayName="Model Relation" Namespace="Luminis.Its.Workbench">
      <BaseClass>
        <DomainClassMoniker Name="ModelType" />
      </BaseClass>
    </DomainClass>
    <DomainClass Id="38ebc3f7-d838-4e0f-b1d6-46fa64a00769" Description="Description for Luminis.Its.Workbench.ModelComplexType" Name="ModelComplexType" DisplayName="Model Complex Type" Namespace="Luminis.Its.Workbench">
      <BaseClass>
        <DomainClassMoniker Name="ModelType" />
      </BaseClass>
    </DomainClass>
    <DomainClass Id="f28c4378-947b-4f0f-8787-3c63b9b1f73b" Description="Description for Luminis.Its.Workbench.ModelEntity" Name="ModelEntity" DisplayName="Model Entity" Namespace="Luminis.Its.Workbench">
      <BaseClass>
        <DomainClassMoniker Name="ModelType" />
      </BaseClass>
    </DomainClass>
  </Classes>
  <Relationships>
    <DomainRelationship Id="dc7ec268-8ceb-48f6-bd60-1b5670cdc908" Description="" Name="ModelRootHasComments" DisplayName="Model Root Has Comments" Namespace="Luminis.Its.Workbench" IsEmbedding="true">
      <Source>
        <DomainRole Id="a8b35e0b-a400-40e5-bb9c-babcd22605f1" Description="" Name="ModelRoot" DisplayName="Model Root" PropertyName="Comments" PropertyDisplayName="Comments">
          <RolePlayer>
            <DomainClassMoniker Name="ModelRoot" />
          </RolePlayer>
        </DomainRole>
      </Source>
      <Target>
        <DomainRole Id="d25b9667-a751-4355-a9e5-d3c5ee72fcc9" Description="" Name="Comment" DisplayName="Comment" PropertyName="ModelRoot" Multiplicity="One" PropagatesDelete="true" PropertyDisplayName="Model Root">
          <RolePlayer>
            <DomainClassMoniker Name="Comment" />
          </RolePlayer>
        </DomainRole>
      </Target>
    </DomainRelationship>
    <DomainRelationship Id="08a6b183-a158-4cde-8b06-c882175e651c" Description="" Name="CommentReferencesSubjects" DisplayName="Comment References Subjects" Namespace="Luminis.Its.Workbench">
      <Source>
        <DomainRole Id="fcb5b49f-d852-4cf9-87d5-19ee3a6bbfe8" Description="" Name="Comment" DisplayName="Comment" PropertyName="Subjects" PropertyDisplayName="Subjects">
          <RolePlayer>
            <DomainClassMoniker Name="Comment" />
          </RolePlayer>
        </DomainRole>
      </Source>
      <Target>
        <DomainRole Id="20841cfe-4c2e-4fb6-9063-ddfd3a12d6af" Description="" Name="Subject" DisplayName="Subject" PropertyName="Comments" PropertyDisplayName="Comments">
          <RolePlayer>
            <DomainClassMoniker Name="ModelType" />
          </RolePlayer>
        </DomainRole>
      </Target>
    </DomainRelationship>
    <DomainRelationship Id="0f794286-0553-4212-9907-2b583ad47b01" Description="Description for Luminis.Its.Workbench.HasObjectModelSpec" Name="HasObjectModelSpec" DisplayName="Has Object Model Spec" Namespace="Luminis.Its.Workbench" IsEmbedding="true">
      <Source>
        <DomainRole Id="25f6da19-3fbe-47d4-8c72-628c8506321e" Description="Description for Luminis.Its.Workbench.HasObjectModelSpec.ModelRoot" Name="ModelRoot" DisplayName="Model Root" PropertyName="ObjectModelSpec" Multiplicity="ZeroOne" PropertyDisplayName="Object Model Spec">
          <RolePlayer>
            <DomainClassMoniker Name="ModelRoot" />
          </RolePlayer>
        </DomainRole>
      </Source>
      <Target>
        <DomainRole Id="02e1293a-27b2-4b89-89a8-85ad86389c3c" Description="Description for Luminis.Its.Workbench.HasObjectModelSpec.ObjectModelSpec" Name="ObjectModelSpec" DisplayName="Object Model Spec" PropertyName="ModelRoot" Multiplicity="One" PropagatesDelete="true" PropagatesCopy="true" PropertyDisplayName="Model Root">
          <RolePlayer>
            <DomainClassMoniker Name="ObjectModelSpec" />
          </RolePlayer>
        </DomainRole>
      </Target>
    </DomainRelationship>
    <DomainRelationship Id="a7a698f4-e5f4-4fc0-ba57-27718c61b447" Description="Description for Luminis.Its.Workbench.ObjectModelSpecHasTypes" Name="ObjectModelSpecHasTypes" DisplayName="Object Model Spec Has Types" Namespace="Luminis.Its.Workbench" IsEmbedding="true">
      <Source>
        <DomainRole Id="067ab529-9b66-4c65-8e84-db424400330f" Description="Description for Luminis.Its.Workbench.ObjectModelSpecHasTypes.ObjectModelSpec" Name="ObjectModelSpec" DisplayName="Object Model Spec" PropertyName="Types" PropertyDisplayName="Types">
          <RolePlayer>
            <DomainClassMoniker Name="ObjectModelSpec" />
          </RolePlayer>
        </DomainRole>
      </Source>
      <Target>
        <DomainRole Id="60c89952-6e1d-4f56-9885-7ba519b57762" Description="Description for Luminis.Its.Workbench.ObjectModelSpecHasTypes.ModelType" Name="ModelType" DisplayName="Model Type" PropertyName="ObjectModelSpec" Multiplicity="One" PropagatesDelete="true" PropagatesCopy="true" PropertyDisplayName="Object Model Spec">
          <RolePlayer>
            <DomainClassMoniker Name="ModelType" />
          </RolePlayer>
        </DomainRole>
      </Target>
    </DomainRelationship>
    <DomainRelationship Id="a5b5cb47-5034-45e0-9d45-a73a1c0f81d6" Description="Description for Luminis.Its.Workbench.HasCaseFileModelSpec" Name="HasCaseFileModelSpec" DisplayName="Has Case File Model Spec" Namespace="Luminis.Its.Workbench" IsEmbedding="true">
      <Source>
        <DomainRole Id="3184ae2a-45b8-40be-8f92-a7fa082f4460" Description="Description for Luminis.Its.Workbench.HasCaseFileModelSpec.ModelRoot" Name="ModelRoot" DisplayName="Model Root" PropertyName="CaseFileModelSpecs" PropertyDisplayName="Case File Model Specs">
          <RolePlayer>
            <DomainClassMoniker Name="ModelRoot" />
          </RolePlayer>
        </DomainRole>
      </Source>
      <Target>
        <DomainRole Id="77969aed-5c59-4a40-995f-e7b360d352f4" Description="Description for Luminis.Its.Workbench.HasCaseFileModelSpec.CaseFileModelSpec" Name="CaseFileModelSpec" DisplayName="Case File Model Spec" PropertyName="ModelRoot" Multiplicity="One" PropagatesDelete="true" PropagatesCopy="true" PropertyDisplayName="Model Root">
          <RolePlayer>
            <DomainClassMoniker Name="CaseFileModelSpec" />
          </RolePlayer>
        </DomainRole>
      </Target>
    </DomainRelationship>
    <DomainRelationship Id="19f6741d-6d88-4934-abb0-9dfcb70ed514" Description="Description for Luminis.Its.Workbench.CaseFileModelSpecHasCaseFileTypes" Name="CaseFileModelSpecHasCaseFileTypes" DisplayName="Case File Model Spec Has Case File Types" Namespace="Luminis.Its.Workbench" IsEmbedding="true">
      <Source>
        <DomainRole Id="cdb0c109-d7b6-446c-95f8-8df1c96accc1" Description="Description for Luminis.Its.Workbench.CaseFileModelSpecHasCaseFileTypes.CaseFileModelSpec" Name="CaseFileModelSpec" DisplayName="Case File Model Spec" PropertyName="CaseFileTypes" PropertyDisplayName="Case File Types">
          <RolePlayer>
            <DomainClassMoniker Name="CaseFileModelSpec" />
          </RolePlayer>
        </DomainRole>
      </Source>
      <Target>
        <DomainRole Id="e3273e02-519f-48b9-a631-68ed023347ed" Description="Description for Luminis.Its.Workbench.CaseFileModelSpecHasCaseFileTypes.CaseFileType" Name="CaseFileType" DisplayName="Case File Type" PropertyName="CaseFileModelSpec" Multiplicity="One" PropagatesDelete="true" PropagatesCopy="true" PropertyDisplayName="Case File Model Spec">
          <RolePlayer>
            <DomainClassMoniker Name="CaseFileType" />
          </RolePlayer>
        </DomainRole>
      </Target>
    </DomainRelationship>
    <DomainRelationship Id="a7204501-b65d-46cb-8524-a0fc1c23f719" Description="Description for Luminis.Its.Workbench.ModelTypeHasAttributes" Name="ModelTypeHasAttributes" DisplayName="Model Type Has Attributes" Namespace="Luminis.Its.Workbench" IsEmbedding="true">
      <Source>
        <DomainRole Id="25aa9c3c-0dfe-417b-b8d0-913264a69ee7" Description="Description for Luminis.Its.Workbench.ModelTypeHasAttributes.ModelType" Name="ModelType" DisplayName="Model Type" PropertyName="Attributes" PropertyDisplayName="Attributes">
          <RolePlayer>
            <DomainClassMoniker Name="ModelType" />
          </RolePlayer>
        </DomainRole>
      </Source>
      <Target>
        <DomainRole Id="a261e219-f3d2-491e-a885-1fed2a0e4b23" Description="Description for Luminis.Its.Workbench.ModelTypeHasAttributes.ModelAttribute" Name="ModelAttribute" DisplayName="Model Attribute" PropertyName="ModelType" Multiplicity="One" PropagatesDelete="true" PropagatesCopy="true" PropertyDisplayName="Model Type">
          <RolePlayer>
            <DomainClassMoniker Name="ModelAttribute" />
          </RolePlayer>
        </DomainRole>
      </Target>
    </DomainRelationship>
    <DomainRelationship Id="553b62d1-12ee-4cbe-8965-0f16b4b1ce70" Description="Description for Luminis.Its.Workbench.EntityHasRelations" Name="EntityHasRelations" DisplayName="Entity Has Relations" Namespace="Luminis.Its.Workbench">
      <Source>
        <DomainRole Id="f254e672-549c-46a8-86d2-2f86ad16b36f" Description="Description for Luminis.Its.Workbench.EntityHasRelations.ModelEntity" Name="ModelEntity" DisplayName="Model Entity" PropertyName="ToRelations" PropertyDisplayName="To Relations">
          <RolePlayer>
            <DomainClassMoniker Name="ModelEntity" />
          </RolePlayer>
        </DomainRole>
      </Source>
      <Target>
        <DomainRole Id="88c6593d-f0c5-43d1-8b42-2bb7460f6a09" Description="Description for Luminis.Its.Workbench.EntityHasRelations.ModelRelation" Name="ModelRelation" DisplayName="Model Relation" PropertyName="Entity" Multiplicity="ZeroOne" PropertyDisplayName="Entity">
          <RolePlayer>
            <DomainClassMoniker Name="ModelRelation" />
          </RolePlayer>
        </DomainRole>
      </Target>
    </DomainRelationship>
    <DomainRelationship Id="e0977312-876a-4959-8d3e-f1597a8114a7" Description="Description for Luminis.Its.Workbench.RelationHasEntity" Name="RelationHasEntity" DisplayName="Relation Has Entity" Namespace="Luminis.Its.Workbench">
      <Source>
        <DomainRole Id="820ff246-df95-4c4c-96e1-f4ad9d22a511" Description="Description for Luminis.Its.Workbench.RelationHasEntity.ModelRelation" Name="ModelRelation" DisplayName="Model Relation" PropertyName="ModelEntity" Multiplicity="ZeroOne" PropertyDisplayName="Model Entity">
          <RolePlayer>
            <DomainClassMoniker Name="ModelRelation" />
          </RolePlayer>
        </DomainRole>
      </Source>
      <Target>
        <DomainRole Id="146e69db-731b-461a-885d-12fb24ee8dbe" Description="Description for Luminis.Its.Workbench.RelationHasEntity.ModelEntity" Name="ModelEntity" DisplayName="Model Entity" PropertyName="Relations" PropertyDisplayName="Relations">
          <RolePlayer>
            <DomainClassMoniker Name="ModelEntity" />
          </RolePlayer>
        </DomainRole>
      </Target>
    </DomainRelationship>
    <DomainRelationship Id="e2f683a2-6528-4d4d-8df7-1e6bffbcbc19" Description="Description for Luminis.Its.Workbench.ModelTypeReferencesModelComplexTypes" Name="ModelTypeReferencesModelComplexTypes" DisplayName="Model Type References Model Complex Types" Namespace="Luminis.Its.Workbench">
      <Properties>
        <DomainProperty Id="79cbb3ff-3b8a-415b-b4b2-471fda182438" Description="Description for Luminis.Its.Workbench.ModelTypeReferencesModelComplexTypes.Container Name" Name="ContainerName" DisplayName="Container Name">
          <Type>
            <ExternalTypeMoniker Name="/System/String" />
          </Type>
        </DomainProperty>
        <DomainProperty Id="92fcc1ca-2a47-44b7-ae05-2e5924af8ebd" Description="Description for Luminis.Its.Workbench.ModelTypeReferencesModelComplexTypes.Is Collection" Name="IsCollection" DisplayName="Is Collection">
          <Type>
            <ExternalTypeMoniker Name="/System/Boolean" />
          </Type>
        </DomainProperty>
      </Properties>
      <Source>
        <DomainRole Id="217eb3dd-4a30-41c8-9b1d-38f91ded6a08" Description="Description for Luminis.Its.Workbench.ModelTypeReferencesModelComplexTypes.ModelType" Name="ModelType" DisplayName="Model Type" PropertyName="ModelComplexTypes" PropertyDisplayName="Model Complex Types">
          <RolePlayer>
            <DomainClassMoniker Name="ModelType" />
          </RolePlayer>
        </DomainRole>
      </Source>
      <Target>
        <DomainRole Id="fdc66735-2226-44ac-8f0c-92d9931baa17" Description="Description for Luminis.Its.Workbench.ModelTypeReferencesModelComplexTypes.ModelComplexType" Name="ModelComplexType" DisplayName="Model Complex Type" PropertyName="ModelTypes" PropertyDisplayName="Model Types">
          <RolePlayer>
            <DomainClassMoniker Name="ModelComplexType" />
          </RolePlayer>
        </DomainRole>
      </Target>
    </DomainRelationship>
    <DomainRelationship Id="1fa088ba-0cea-4882-9e27-0b1f81cc7b85" Description="Description for Luminis.Its.Workbench.CaseFileEntityReferencesChildCaseFileRelations" Name="CaseFileEntityReferencesChildCaseFileRelations" DisplayName="Case File Entity References Child Case File Relations" Namespace="Luminis.Its.Workbench">
      <Source>
        <DomainRole Id="b239e0e0-1b34-4e6d-97dd-ce158cf04726" Description="Description for Luminis.Its.Workbench.CaseFileEntityReferencesChildCaseFileRelations.CaseFileEntity" Name="CaseFileEntity" DisplayName="Case File Entity" PropertyName="ChildCaseFileRelations" PropertyDisplayName="Child Case File Relations">
          <RolePlayer>
            <DomainClassMoniker Name="CaseFileEntity" />
          </RolePlayer>
        </DomainRole>
      </Source>
      <Target>
        <DomainRole Id="77810259-2640-4535-9571-a266a62fce99" Description="Description for Luminis.Its.Workbench.CaseFileEntityReferencesChildCaseFileRelations.CaseFileRelation" Name="CaseFileRelation" DisplayName="Case File Relation" PropertyName="ParentCaseFileEntity" Multiplicity="ZeroOne" PropertyDisplayName="Parent Case File Entity">
          <RolePlayer>
            <DomainClassMoniker Name="CaseFileRelation" />
          </RolePlayer>
        </DomainRole>
      </Target>
    </DomainRelationship>
    <DomainRelationship Id="cbe91084-489a-4e48-8edb-5b7017612dd1" Description="Description for Luminis.Its.Workbench.CaseFileRelationReferencesChildCaseFileEntity" Name="CaseFileRelationReferencesChildCaseFileEntity" DisplayName="Case File Relation References Child Case File Entity" Namespace="Luminis.Its.Workbench">
      <Source>
        <DomainRole Id="b30aee02-a4cb-4032-88f0-868156353d73" Description="Description for Luminis.Its.Workbench.CaseFileRelationReferencesChildCaseFileEntity.CaseFileRelation" Name="CaseFileRelation" DisplayName="Case File Relation" PropertyName="ChildCaseFileEntity" Multiplicity="ZeroOne" PropertyDisplayName="Child Case File Entity">
          <RolePlayer>
            <DomainClassMoniker Name="CaseFileRelation" />
          </RolePlayer>
        </DomainRole>
      </Source>
      <Target>
        <DomainRole Id="1fb8e86f-2d88-484e-a9df-7d6d7136b674" Description="Description for Luminis.Its.Workbench.CaseFileRelationReferencesChildCaseFileEntity.CaseFileEntity" Name="CaseFileEntity" DisplayName="Case File Entity" PropertyName="ParentCaseFileRelation" Multiplicity="ZeroOne" PropertyDisplayName="Parent Case File Relation">
          <RolePlayer>
            <DomainClassMoniker Name="CaseFileEntity" />
          </RolePlayer>
        </DomainRole>
      </Target>
    </DomainRelationship>
    <DomainRelationship Id="e154bce2-7a94-46ee-b82c-8b7d0b00a0e4" Description="Description for Luminis.Its.Workbench.RootCaseFileEntity" Name="RootCaseFileEntity" DisplayName="Root Case File Entity" Namespace="Luminis.Its.Workbench">
      <Source>
        <DomainRole Id="9e2b4080-67b6-42f7-be75-8716f5c32fb3" Description="Description for Luminis.Its.Workbench.RootCaseFileEntity.CaseFileModelSpec" Name="CaseFileModelSpec" DisplayName="Case File Model Spec" PropertyName="CaseFileRootEntity" Multiplicity="ZeroOne" PropertyDisplayName="Case File Root Entity">
          <RolePlayer>
            <DomainClassMoniker Name="CaseFileModelSpec" />
          </RolePlayer>
        </DomainRole>
      </Source>
      <Target>
        <DomainRole Id="64f32d58-e9a3-4cdd-ba4f-996fb3878d8e" Description="Description for Luminis.Its.Workbench.RootCaseFileEntity.CaseFileEntity" Name="CaseFileEntity" DisplayName="Case File Entity" PropertyName="CaseFileModelSpecs" PropertyDisplayName="Case File Model Specs">
          <RolePlayer>
            <DomainClassMoniker Name="CaseFileEntity" />
          </RolePlayer>
        </DomainRole>
      </Target>
    </DomainRelationship>
    <DomainRelationship Id="f96fbfd4-0f6f-48c1-be57-975c8c6521ad" Description="Description for Luminis.Its.Workbench.CaseFileEntityReferencesModelEntity" Name="CaseFileEntityReferencesModelEntity" DisplayName="Case File Entity References Model Entity" Namespace="Luminis.Its.Workbench">
      <Source>
        <DomainRole Id="ee9311cb-a14b-4f71-a4c6-ce1e025706ed" Description="Description for Luminis.Its.Workbench.CaseFileEntityReferencesModelEntity.CaseFileEntity" Name="CaseFileEntity" DisplayName="Case File Entity" PropertyName="ModelEntity" Multiplicity="ZeroOne" PropertyDisplayName="Model Entity">
          <RolePlayer>
            <DomainClassMoniker Name="CaseFileEntity" />
          </RolePlayer>
        </DomainRole>
      </Source>
      <Target>
        <DomainRole Id="5069f59a-5c87-4c9c-a8c5-04c5190a6b44" Description="Description for Luminis.Its.Workbench.CaseFileEntityReferencesModelEntity.ModelEntity" Name="ModelEntity" DisplayName="Model Entity" PropertyName="CaseFileEntities" PropertyDisplayName="Case File Entities">
          <RolePlayer>
            <DomainClassMoniker Name="ModelEntity" />
          </RolePlayer>
        </DomainRole>
      </Target>
    </DomainRelationship>
    <DomainRelationship Id="0b03db2e-eb8e-4bbc-807f-4804d9ca4a40" Description="Description for Luminis.Its.Workbench.CaseFileRelationReferencesModelRelation" Name="CaseFileRelationReferencesModelRelation" DisplayName="Case File Relation References Model Relation" Namespace="Luminis.Its.Workbench">
      <Source>
        <DomainRole Id="5ce92069-f720-4794-bb75-dd5642b05880" Description="Description for Luminis.Its.Workbench.CaseFileRelationReferencesModelRelation.CaseFileRelation" Name="CaseFileRelation" DisplayName="Case File Relation" PropertyName="ModelRelation" Multiplicity="ZeroOne" PropertyDisplayName="Model Relation">
          <RolePlayer>
            <DomainClassMoniker Name="CaseFileRelation" />
          </RolePlayer>
        </DomainRole>
      </Source>
      <Target>
        <DomainRole Id="ee724914-abe1-4c52-acac-bca400956557" Description="Description for Luminis.Its.Workbench.CaseFileRelationReferencesModelRelation.ModelRelation" Name="ModelRelation" DisplayName="Model Relation" PropertyName="CaseFileRelations" PropertyDisplayName="Case File Relations">
          <RolePlayer>
            <DomainClassMoniker Name="ModelRelation" />
          </RolePlayer>
        </DomainRole>
      </Target>
    </DomainRelationship>
  </Relationships>
  <Types>
    <ExternalType Name="DateTime" Namespace="System" />
    <ExternalType Name="String" Namespace="System" />
    <ExternalType Name="Int16" Namespace="System" />
    <ExternalType Name="Int32" Namespace="System" />
    <ExternalType Name="Int64" Namespace="System" />
    <ExternalType Name="UInt16" Namespace="System" />
    <ExternalType Name="UInt32" Namespace="System" />
    <ExternalType Name="UInt64" Namespace="System" />
    <ExternalType Name="SByte" Namespace="System" />
    <ExternalType Name="Byte" Namespace="System" />
    <ExternalType Name="Double" Namespace="System" />
    <ExternalType Name="Single" Namespace="System" />
    <ExternalType Name="Guid" Namespace="System" />
    <ExternalType Name="Boolean" Namespace="System" />
    <ExternalType Name="Char" Namespace="System" />
  </Types>
  <Shapes>
    <CompartmentShape Id="d8cab3df-3d0a-4f9b-8427-68e4e5dbb396" Description="" Name="ModelEntityShape" DisplayName="Model Entity Shape" Namespace="Luminis.Its.Workbench" FixedTooltipText="Model Entity Shape" FillColor="Chocolate" InitialHeight="0.3" OutlineThickness="0.01" Geometry="RoundedRectangle" DefaultExpandCollapseState="Collapsed">
      <ShapeHasDecorators Position="InnerTopCenter" HorizontalOffset="0" VerticalOffset="0">
        <TextDecorator Name="Name" DisplayName="Name" DefaultText="Name" />
      </ShapeHasDecorators>
      <ShapeHasDecorators Position="InnerTopRight" HorizontalOffset="0" VerticalOffset="0">
        <ExpandCollapseDecorator Name="ExpandCollapse" DisplayName="Expand Collapse" />
      </ShapeHasDecorators>
      <Compartment TitleFillColor="235, 235, 235" Name="AttributesCompartment" Title="Attributes" />
    </CompartmentShape>
    <GeometryShape Id="335c5796-5cbd-4a31-adf6-cff41fd272f9" Description="" Name="CommentBoxShape" DisplayName="Comment Box Shape" Namespace="Luminis.Its.Workbench" FixedTooltipText="Comment Box Shape" FillColor="255, 255, 204" OutlineColor="204, 204, 102" InitialHeight="0.3" OutlineThickness="0.01" FillGradientMode="None" Geometry="Rectangle">
      <ShapeHasDecorators Position="Center" HorizontalOffset="0" VerticalOffset="0">
        <TextDecorator Name="Comment" DisplayName="Comment" DefaultText="BusinessRulesShapeNameDecorator" />
      </ShapeHasDecorators>
    </GeometryShape>
    <SwimLane Id="5ea986a3-d593-44f8-85d8-4aba3575c12c" Description="Description for Luminis.Its.Workbench.ObjectModelLane" Name="ObjectModelLane" DisplayName="Object Model Lane" Namespace="Luminis.Its.Workbench" FixedTooltipText="Object Model Lane" InitialWidth="0" InitialHeight="0">
      <Decorators>
        <SwimLaneHasDecorators Position="InnerTopLeft" HorizontalOffset="0" VerticalOffset="0">
          <TextDecorator Name="Title" DisplayName="Title" DefaultText="Title" />
        </SwimLaneHasDecorators>
      </Decorators>
    </SwimLane>
    <SwimLane Id="925988cb-2a9a-47df-8bc5-f3fcb2eeb008" Description="Description for Luminis.Its.Workbench.CaseFileSpecLane" Name="CaseFileSpecLane" DisplayName="Case File Spec Lane" Namespace="Luminis.Its.Workbench" FixedTooltipText="Case File Spec Lane" BodyFillColor="Silver" InitialWidth="0" InitialHeight="0">
      <Decorators>
        <SwimLaneHasDecorators Position="InnerTopLeft" HorizontalOffset="0" VerticalOffset="0">
          <TextDecorator Name="Title" DisplayName="Title" DefaultText="Title" />
        </SwimLaneHasDecorators>
      </Decorators>
    </SwimLane>
    <GeometryShape Id="524d1a89-9984-4063-8172-aa67c906de7a" Description="Description for Luminis.Its.Workbench.CaseFileEntityShape" Name="CaseFileEntityShape" DisplayName="Case File Entity Shape" Namespace="Luminis.Its.Workbench" GeneratesDoubleDerived="true" FixedTooltipText="Case File Entity Shape" FillColor="Chocolate" InitialHeight="0.3" OutlineThickness="0.01" Geometry="RoundedRectangle">
      <ShapeHasDecorators Position="InnerTopLeft" HorizontalOffset="0" VerticalOffset="0">
        <TextDecorator Name="Title" DisplayName="Title" DefaultText="Title" />
      </ShapeHasDecorators>
    </GeometryShape>
    <GeometryShape Id="594a10ca-ec3b-4f04-a2d9-6e96cb1b3dc4" Description="Description for Luminis.Its.Workbench.CaseFileRelationShape" Name="CaseFileRelationShape" DisplayName="Case File Relation Shape" Namespace="Luminis.Its.Workbench" FixedTooltipText="Case File Relation Shape" FillColor="SkyBlue" InitialHeight="0.3" OutlineThickness="0.01" Geometry="RoundedRectangle">
      <ShapeHasDecorators Position="InnerTopLeft" HorizontalOffset="0" VerticalOffset="0">
        <TextDecorator Name="Title" DisplayName="Title" DefaultText="Title" />
      </ShapeHasDecorators>
    </GeometryShape>
    <CompartmentShape Id="eb9ee564-64b1-4b3c-8bb9-42a3df45016a" Description="Description for Luminis.Its.Workbench.ModelRelationShape" Name="ModelRelationShape" DisplayName="Model Relation Shape" Namespace="Luminis.Its.Workbench" FixedTooltipText="Model Relation Shape" FillColor="SkyBlue" InitialHeight="0.3" OutlineThickness="0.01" Geometry="RoundedRectangle" DefaultExpandCollapseState="Collapsed">
      <ShapeHasDecorators Position="InnerTopLeft" HorizontalOffset="0" VerticalOffset="0">
        <TextDecorator Name="Name" DisplayName="Name" DefaultText="Name" />
      </ShapeHasDecorators>
      <ShapeHasDecorators Position="InnerTopRight" HorizontalOffset="0" VerticalOffset="0">
        <ExpandCollapseDecorator Name="ExpandCollapse" DisplayName="Expand Collapse" />
      </ShapeHasDecorators>
      <Compartment Name="AttributesCompartment" Title="Attributes" />
    </CompartmentShape>
    <CompartmentShape Id="a1da140c-3854-42fd-abd6-9239454c20ad" Description="Description for Luminis.Its.Workbench.ModelComplexTypeShape" Name="ModelComplexTypeShape" DisplayName="Model Complex Type Shape" Namespace="Luminis.Its.Workbench" FixedTooltipText="Model Complex Type Shape" InitialHeight="0.3" OutlineThickness="0.01" Geometry="RoundedRectangle" DefaultExpandCollapseState="Collapsed">
      <ShapeHasDecorators Position="InnerTopLeft" HorizontalOffset="0" VerticalOffset="0">
        <TextDecorator Name="Name" DisplayName="Name" DefaultText="Name" />
      </ShapeHasDecorators>
      <ShapeHasDecorators Position="InnerTopRight" HorizontalOffset="0" VerticalOffset="0">
        <ExpandCollapseDecorator Name="ExpandCollapse" DisplayName="Expand Collapse" />
      </ShapeHasDecorators>
      <Compartment Name="AttributesCompartment" Title="Attributes" />
    </CompartmentShape>
  </Shapes>
  <Connectors>
    <Connector Id="f0290a86-3f68-45c7-9fab-e84a6d9df108" Description="" Name="EntityConnector" DisplayName="Entity Connector" Namespace="Luminis.Its.Workbench" FixedTooltipText="Entity Connector" Color="113, 111, 110" TargetEndStyle="EmptyArrow" Thickness="0.02" />
    <Connector Id="e6e9ce90-db88-49de-b1bf-d6a82433af56" Description="" Name="CommentConnector" DisplayName="Comment Connector" Namespace="Luminis.Its.Workbench" FixedTooltipText="Comment Connector" Color="113, 111, 110" DashStyle="Dot" Thickness="0.01" RoutingStyle="Straight" />
    <Connector Id="d99fc279-6b70-48aa-b7ed-43a6872e1a79" Description="Description for Luminis.Its.Workbench.CaseFileElementConnector" Name="CaseFileElementConnector" DisplayName="Case File Element Connector" Namespace="Luminis.Its.Workbench" FixedTooltipText="Case File Element Connector" Thickness="0.01" />
    <Connector Id="f0878d75-43f6-46fa-b58f-a0a23b6a7855" Description="Description for Luminis.Its.Workbench.CaseFileToModelConnector" Name="CaseFileToModelConnector" DisplayName="Case File To Model Connector" Namespace="Luminis.Its.Workbench" FixedTooltipText="Case File To Model Connector" TextColor="Gray" DashStyle="Dot" />
    <Connector Id="8c8ccd60-725b-48ae-ada7-07a1fe6d88b9" Description="Description for Luminis.Its.Workbench.RelationConnector" Name="RelationConnector" DisplayName="Relation Connector" Namespace="Luminis.Its.Workbench" FixedTooltipText="Relation Connector" TargetEndStyle="EmptyArrow" Thickness="0.02" />
    <Connector Id="0c827b09-b586-4489-9853-75b0c697ca6d" Description="Description for Luminis.Its.Workbench.ComplexTypeConnector" Name="ComplexTypeConnector" DisplayName="Complex Type Connector" Namespace="Luminis.Its.Workbench" FixedTooltipText="Complex Type Connector" SourceEndStyle="FilledDiamond" TargetEndStyle="EmptyArrow" Thickness="0.02">
      <ConnectorHasDecorators Position="SourceTop" OffsetFromShape="0" OffsetFromLine="0">
        <TextDecorator Name="Name" DisplayName="Name" DefaultText="Name" />
      </ConnectorHasDecorators>
    </Connector>
  </Connectors>
  <XmlSerializationBehavior Name="WorkbenchSerializationBehavior" Namespace="Luminis.Its.Workbench">
    <ClassData>
      <XmlClassData TypeName="NamedElement" MonikerAttributeName="" MonikerElementName="namedElementMoniker" ElementName="namedElement" MonikerTypeName="NamedElementMoniker">
        <DomainClassMoniker Name="NamedElement" />
        <ElementData>
          <XmlPropertyData XmlName="name" IsMonikerKey="true">
            <DomainPropertyMoniker Name="NamedElement/Name" />
          </XmlPropertyData>
        </ElementData>
      </XmlClassData>
      <XmlClassData TypeName="ModelRootHasComments" MonikerAttributeName="" MonikerElementName="modelRootHasCommentsMoniker" ElementName="modelRootHasComments" MonikerTypeName="ModelRootHasCommentsMoniker">
        <DomainRelationshipMoniker Name="ModelRootHasComments" />
      </XmlClassData>
      <XmlClassData TypeName="CommentReferencesSubjects" MonikerAttributeName="" MonikerElementName="commentReferencesSubjectsMoniker" ElementName="commentReferencesSubjects" MonikerTypeName="CommentReferencesSubjectsMoniker">
        <DomainRelationshipMoniker Name="CommentReferencesSubjects" />
      </XmlClassData>
      <XmlClassData TypeName="ModelRoot" MonikerAttributeName="" MonikerElementName="modelRootMoniker" ElementName="modelRoot" MonikerTypeName="ModelRootMoniker">
        <DomainClassMoniker Name="ModelRoot" />
        <ElementData>
          <XmlRelationshipData RoleElementName="comments">
            <DomainRelationshipMoniker Name="ModelRootHasComments" />
          </XmlRelationshipData>
          <XmlRelationshipData RoleElementName="objectModelSpec">
            <DomainRelationshipMoniker Name="HasObjectModelSpec" />
          </XmlRelationshipData>
          <XmlRelationshipData RoleElementName="caseFileModelSpecs">
            <DomainRelationshipMoniker Name="HasCaseFileModelSpec" />
          </XmlRelationshipData>
        </ElementData>
      </XmlClassData>
      <XmlClassData TypeName="ModelAttribute" MonikerAttributeName="" MonikerElementName="modelAttributeMoniker" ElementName="modelAttribute" MonikerTypeName="ModelAttributeMoniker">
        <DomainClassMoniker Name="ModelAttribute" />
        <ElementData>
          <XmlPropertyData XmlName="type">
            <DomainPropertyMoniker Name="ModelAttribute/Type" />
          </XmlPropertyData>
          <XmlPropertyData XmlName="initialValue">
            <DomainPropertyMoniker Name="ModelAttribute/InitialValue" />
          </XmlPropertyData>
          <XmlPropertyData XmlName="required">
            <DomainPropertyMoniker Name="ModelAttribute/Required" />
          </XmlPropertyData>
        </ElementData>
      </XmlClassData>
      <XmlClassData TypeName="Comment" MonikerAttributeName="" SerializeId="true" MonikerElementName="commentMoniker" ElementName="comment" MonikerTypeName="CommentMoniker">
        <DomainClassMoniker Name="Comment" />
        <ElementData>
          <XmlPropertyData XmlName="text">
            <DomainPropertyMoniker Name="Comment/Text" />
          </XmlPropertyData>
          <XmlRelationshipData RoleElementName="subjects">
            <DomainRelationshipMoniker Name="CommentReferencesSubjects" />
          </XmlRelationshipData>
        </ElementData>
      </XmlClassData>
      <XmlClassData TypeName="ModelType" MonikerAttributeName="" MonikerElementName="modelTypeMoniker" ElementName="modelType" MonikerTypeName="ModelTypeMoniker">
        <DomainClassMoniker Name="ModelType" />
        <ElementData>
          <XmlRelationshipData RoleElementName="attributes">
            <DomainRelationshipMoniker Name="ModelTypeHasAttributes" />
          </XmlRelationshipData>
          <XmlRelationshipData UseFullForm="true" RoleElementName="modelComplexTypes">
            <DomainRelationshipMoniker Name="ModelTypeReferencesModelComplexTypes" />
          </XmlRelationshipData>
        </ElementData>
      </XmlClassData>
      <XmlClassData TypeName="ObjectModelElement" MonikerAttributeName="" MonikerElementName="objectModelElementMoniker" ElementName="objectModelElement" MonikerTypeName="ObjectModelElementMoniker">
        <DomainClassMoniker Name="ObjectModelElement" />
        <ElementData>
          <XmlPropertyData XmlName="description">
            <DomainPropertyMoniker Name="ObjectModelElement/Description" />
          </XmlPropertyData>
        </ElementData>
      </XmlClassData>
      <XmlClassData TypeName="ModelEntityShape" MonikerAttributeName="" MonikerElementName="modelEntityShapeMoniker" ElementName="modelEntityShape" MonikerTypeName="ModelEntityShapeMoniker">
        <CompartmentShapeMoniker Name="ModelEntityShape" />
      </XmlClassData>
      <XmlClassData TypeName="CommentBoxShape" MonikerAttributeName="" MonikerElementName="commentBoxShapeMoniker" ElementName="commentBoxShape" MonikerTypeName="CommentBoxShapeMoniker">
        <GeometryShapeMoniker Name="CommentBoxShape" />
      </XmlClassData>
      <XmlClassData TypeName="EntityConnector" MonikerAttributeName="" MonikerElementName="entityConnectorMoniker" ElementName="entityConnector" MonikerTypeName="EntityConnectorMoniker">
        <ConnectorMoniker Name="EntityConnector" />
      </XmlClassData>
      <XmlClassData TypeName="CommentConnector" MonikerAttributeName="" MonikerElementName="commentConnectorMoniker" ElementName="commentConnector" MonikerTypeName="CommentConnectorMoniker">
        <ConnectorMoniker Name="CommentConnector" />
      </XmlClassData>
      <XmlClassData TypeName="ClassDiagram" MonikerAttributeName="" MonikerElementName="classDiagramMoniker" ElementName="classDiagram" MonikerTypeName="ClassDiagramMoniker">
        <DiagramMoniker Name="ClassDiagram" />
      </XmlClassData>
      <XmlClassData TypeName="ObjectModelSpec" MonikerAttributeName="" MonikerElementName="objectModelSpecMoniker" ElementName="objectModelSpec" MonikerTypeName="ObjectModelSpecMoniker">
        <DomainClassMoniker Name="ObjectModelSpec" />
        <ElementData>
          <XmlRelationshipData RoleElementName="types">
            <DomainRelationshipMoniker Name="ObjectModelSpecHasTypes" />
          </XmlRelationshipData>
          <XmlPropertyData XmlName="self">
            <DomainPropertyMoniker Name="ObjectModelSpec/Self" />
          </XmlPropertyData>
        </ElementData>
      </XmlClassData>
      <XmlClassData TypeName="HasObjectModelSpec" MonikerAttributeName="" MonikerElementName="hasObjectModelSpecMoniker" ElementName="hasObjectModelSpec" MonikerTypeName="HasObjectModelSpecMoniker">
        <DomainRelationshipMoniker Name="HasObjectModelSpec" />
      </XmlClassData>
      <XmlClassData TypeName="ObjectModelLane" MonikerAttributeName="" MonikerElementName="objectModelLaneMoniker" ElementName="objectModelLane" MonikerTypeName="ObjectModelLaneMoniker">
        <SwimLaneMoniker Name="ObjectModelLane" />
      </XmlClassData>
      <XmlClassData TypeName="ObjectModelSpecHasTypes" MonikerAttributeName="" MonikerElementName="objectModelSpecHasTypesMoniker" ElementName="objectModelSpecHasTypes" MonikerTypeName="ObjectModelSpecHasTypesMoniker">
        <DomainRelationshipMoniker Name="ObjectModelSpecHasTypes" />
      </XmlClassData>
      <XmlClassData TypeName="CaseFileModelSpec" MonikerAttributeName="" MonikerElementName="caseFileModelSpecMoniker" ElementName="caseFileModelSpec" MonikerTypeName="CaseFileModelSpecMoniker">
        <DomainClassMoniker Name="CaseFileModelSpec" />
        <ElementData>
          <XmlRelationshipData RoleElementName="caseFileTypes">
            <DomainRelationshipMoniker Name="CaseFileModelSpecHasCaseFileTypes" />
          </XmlRelationshipData>
          <XmlPropertyData XmlName="uriTemplate">
            <DomainPropertyMoniker Name="CaseFileModelSpec/UriTemplate" />
          </XmlPropertyData>
          <XmlRelationshipData RoleElementName="caseFileRootEntity">
            <DomainRelationshipMoniker Name="RootCaseFileEntity" />
          </XmlRelationshipData>
          <XmlPropertyData XmlName="self">
            <DomainPropertyMoniker Name="CaseFileModelSpec/Self" />
          </XmlPropertyData>
          <XmlPropertyData XmlName="objectModelSpec">
            <DomainPropertyMoniker Name="CaseFileModelSpec/ObjectModelSpec" />
          </XmlPropertyData>
        </ElementData>
      </XmlClassData>
      <XmlClassData TypeName="HasCaseFileModelSpec" MonikerAttributeName="" MonikerElementName="hasCaseFileModelSpecMoniker" ElementName="hasCaseFileModelSpec" MonikerTypeName="HasCaseFileModelSpecMoniker">
        <DomainRelationshipMoniker Name="HasCaseFileModelSpec" />
      </XmlClassData>
      <XmlClassData TypeName="CaseFileSpecLane" MonikerAttributeName="" MonikerElementName="caseFileSpecLaneMoniker" ElementName="caseFileSpecLane" MonikerTypeName="CaseFileSpecLaneMoniker">
        <SwimLaneMoniker Name="CaseFileSpecLane" />
      </XmlClassData>
      <XmlClassData TypeName="CaseFileEntityShape" MonikerAttributeName="" MonikerElementName="caseFileEntityShapeMoniker" ElementName="caseFileEntityShape" MonikerTypeName="CaseFileEntityShapeMoniker">
        <GeometryShapeMoniker Name="CaseFileEntityShape" />
      </XmlClassData>
      <XmlClassData TypeName="CaseFileRelationShape" MonikerAttributeName="" MonikerElementName="caseFileRelationShapeMoniker" ElementName="caseFileRelationShape" MonikerTypeName="CaseFileRelationShapeMoniker">
        <GeometryShapeMoniker Name="CaseFileRelationShape" />
      </XmlClassData>
      <XmlClassData TypeName="CaseFileElementConnector" MonikerAttributeName="" MonikerElementName="caseFileElementConnectorMoniker" ElementName="caseFileElementConnector" MonikerTypeName="CaseFileElementConnectorMoniker">
        <ConnectorMoniker Name="CaseFileElementConnector" />
      </XmlClassData>
      <XmlClassData TypeName="CaseFileType" MonikerAttributeName="" MonikerElementName="caseFileTypeMoniker" ElementName="caseFileType" MonikerTypeName="CaseFileTypeMoniker">
        <DomainClassMoniker Name="CaseFileType" />
      </XmlClassData>
      <XmlClassData TypeName="CaseFileEntity" MonikerAttributeName="" MonikerElementName="caseFileEntityMoniker" ElementName="caseFileEntity" MonikerTypeName="CaseFileEntityMoniker">
        <DomainClassMoniker Name="CaseFileEntity" />
        <ElementData>
          <XmlRelationshipData RoleElementName="childCaseFileRelations">
            <DomainRelationshipMoniker Name="CaseFileEntityReferencesChildCaseFileRelations" />
          </XmlRelationshipData>
          <XmlRelationshipData RoleElementName="modelEntity">
            <DomainRelationshipMoniker Name="CaseFileEntityReferencesModelEntity" />
          </XmlRelationshipData>
        </ElementData>
      </XmlClassData>
      <XmlClassData TypeName="CaseFileRelation" MonikerAttributeName="" MonikerElementName="caseFileRelationMoniker" ElementName="caseFileRelation" MonikerTypeName="CaseFileRelationMoniker">
        <DomainClassMoniker Name="CaseFileRelation" />
        <ElementData>
          <XmlRelationshipData RoleElementName="childCaseFileEntity">
            <DomainRelationshipMoniker Name="CaseFileRelationReferencesChildCaseFileEntity" />
          </XmlRelationshipData>
          <XmlRelationshipData RoleElementName="modelRelation">
            <DomainRelationshipMoniker Name="CaseFileRelationReferencesModelRelation" />
          </XmlRelationshipData>
        </ElementData>
      </XmlClassData>
      <XmlClassData TypeName="CaseFileModelSpecHasCaseFileTypes" MonikerAttributeName="" MonikerElementName="caseFileModelSpecHasCaseFileTypesMoniker" ElementName="caseFileModelSpecHasCaseFileTypes" MonikerTypeName="CaseFileModelSpecHasCaseFileTypesMoniker">
        <DomainRelationshipMoniker Name="CaseFileModelSpecHasCaseFileTypes" />
      </XmlClassData>
      <XmlClassData TypeName="CaseFileToModelConnector" MonikerAttributeName="" MonikerElementName="caseFileToModelConnectorMoniker" ElementName="caseFileToModelConnector" MonikerTypeName="CaseFileToModelConnectorMoniker">
        <ConnectorMoniker Name="CaseFileToModelConnector" />
      </XmlClassData>
      <XmlClassData TypeName="ModelRelation" MonikerAttributeName="" MonikerElementName="modelRelationMoniker" ElementName="modelRelation" MonikerTypeName="ModelRelationMoniker">
        <DomainClassMoniker Name="ModelRelation" />
        <ElementData>
          <XmlRelationshipData RoleElementName="modelEntity">
            <DomainRelationshipMoniker Name="RelationHasEntity" />
          </XmlRelationshipData>
        </ElementData>
      </XmlClassData>
      <XmlClassData TypeName="ModelRelationShape" MonikerAttributeName="" MonikerElementName="modelRelationShapeMoniker" ElementName="modelRelationShape" MonikerTypeName="ModelRelationShapeMoniker">
        <CompartmentShapeMoniker Name="ModelRelationShape" />
      </XmlClassData>
      <XmlClassData TypeName="ModelComplexType" MonikerAttributeName="" MonikerElementName="modelComplexTypeMoniker" ElementName="modelComplexType" MonikerTypeName="ModelComplexTypeMoniker">
        <DomainClassMoniker Name="ModelComplexType" />
      </XmlClassData>
      <XmlClassData TypeName="ModelComplexTypeShape" MonikerAttributeName="" MonikerElementName="modelComplexTypeShapeMoniker" ElementName="modelComplexTypeShape" MonikerTypeName="ModelComplexTypeShapeMoniker">
        <CompartmentShapeMoniker Name="ModelComplexTypeShape" />
      </XmlClassData>
      <XmlClassData TypeName="ModelEntity" MonikerAttributeName="" MonikerElementName="modelEntityMoniker" ElementName="modelEntity" MonikerTypeName="ModelEntityMoniker">
        <DomainClassMoniker Name="ModelEntity" />
        <ElementData>
          <XmlRelationshipData RoleElementName="toRelations">
            <DomainRelationshipMoniker Name="EntityHasRelations" />
          </XmlRelationshipData>
        </ElementData>
      </XmlClassData>
      <XmlClassData TypeName="ModelTypeHasAttributes" MonikerAttributeName="" MonikerElementName="modelTypeHasAttributesMoniker" ElementName="modelTypeHasAttributes" MonikerTypeName="ModelTypeHasAttributesMoniker">
        <DomainRelationshipMoniker Name="ModelTypeHasAttributes" />
      </XmlClassData>
      <XmlClassData TypeName="EntityHasRelations" MonikerAttributeName="" MonikerElementName="entityHasRelationsMoniker" ElementName="entityHasRelations" MonikerTypeName="EntityHasRelationsMoniker">
        <DomainRelationshipMoniker Name="EntityHasRelations" />
      </XmlClassData>
      <XmlClassData TypeName="RelationHasEntity" MonikerAttributeName="" MonikerElementName="relationHasEntityMoniker" ElementName="relationHasEntity" MonikerTypeName="RelationHasEntityMoniker">
        <DomainRelationshipMoniker Name="RelationHasEntity" />
      </XmlClassData>
      <XmlClassData TypeName="RelationConnector" MonikerAttributeName="" MonikerElementName="relationConnectorMoniker" ElementName="relationConnector" MonikerTypeName="RelationConnectorMoniker">
        <ConnectorMoniker Name="RelationConnector" />
      </XmlClassData>
      <XmlClassData TypeName="ModelTypeReferencesModelComplexTypes" MonikerAttributeName="" MonikerElementName="modelTypeReferencesModelComplexTypesMoniker" ElementName="modelTypeReferencesModelComplexTypes" MonikerTypeName="ModelTypeReferencesModelComplexTypesMoniker">
        <DomainRelationshipMoniker Name="ModelTypeReferencesModelComplexTypes" />
        <ElementData>
          <XmlPropertyData XmlName="containerName">
            <DomainPropertyMoniker Name="ModelTypeReferencesModelComplexTypes/ContainerName" />
          </XmlPropertyData>
          <XmlPropertyData XmlName="isCollection">
            <DomainPropertyMoniker Name="ModelTypeReferencesModelComplexTypes/IsCollection" />
          </XmlPropertyData>
        </ElementData>
      </XmlClassData>
      <XmlClassData TypeName="ComplexTypeConnector" MonikerAttributeName="" MonikerElementName="complexTypeConnectorMoniker" ElementName="complexTypeConnector" MonikerTypeName="ComplexTypeConnectorMoniker">
        <ConnectorMoniker Name="ComplexTypeConnector" />
      </XmlClassData>
      <XmlClassData TypeName="CaseFileEntityReferencesChildCaseFileRelations" MonikerAttributeName="" MonikerElementName="caseFileEntityReferencesChildCaseFileRelationsMoniker" ElementName="caseFileEntityReferencesChildCaseFileRelations" MonikerTypeName="CaseFileEntityReferencesChildCaseFileRelationsMoniker">
        <DomainRelationshipMoniker Name="CaseFileEntityReferencesChildCaseFileRelations" />
      </XmlClassData>
      <XmlClassData TypeName="CaseFileRelationReferencesChildCaseFileEntity" MonikerAttributeName="" MonikerElementName="caseFileRelationReferencesChildCaseFileEntityMoniker" ElementName="caseFileRelationReferencesChildCaseFileEntity" MonikerTypeName="CaseFileRelationReferencesChildCaseFileEntityMoniker">
        <DomainRelationshipMoniker Name="CaseFileRelationReferencesChildCaseFileEntity" />
      </XmlClassData>
      <XmlClassData TypeName="RootCaseFileEntity" MonikerAttributeName="" MonikerElementName="rootCaseFileEntityMoniker" ElementName="rootCaseFileEntity" MonikerTypeName="RootCaseFileEntityMoniker">
        <DomainRelationshipMoniker Name="RootCaseFileEntity" />
      </XmlClassData>
      <XmlClassData TypeName="CaseFileEntityReferencesModelEntity" MonikerAttributeName="" MonikerElementName="caseFileEntityReferencesModelEntityMoniker" ElementName="caseFileEntityReferencesModelEntity" MonikerTypeName="CaseFileEntityReferencesModelEntityMoniker">
        <DomainRelationshipMoniker Name="CaseFileEntityReferencesModelEntity" />
      </XmlClassData>
      <XmlClassData TypeName="CaseFileRelationReferencesModelRelation" MonikerAttributeName="" MonikerElementName="caseFileRelationReferencesModelRelationMoniker" ElementName="caseFileRelationReferencesModelRelation" MonikerTypeName="CaseFileRelationReferencesModelRelationMoniker">
        <DomainRelationshipMoniker Name="CaseFileRelationReferencesModelRelation" />
      </XmlClassData>
    </ClassData>
  </XmlSerializationBehavior>
  <ExplorerBehavior Name="WorkbenchExplorer" />
  <ConnectionBuilders>
    <ConnectionBuilder Name="CommentReferencesSubjectsBuilder">
      <LinkConnectDirective>
        <DomainRelationshipMoniker Name="CommentReferencesSubjects" />
        <SourceDirectives>
          <RolePlayerConnectDirective>
            <AcceptingClass>
              <DomainClassMoniker Name="Comment" />
            </AcceptingClass>
          </RolePlayerConnectDirective>
        </SourceDirectives>
        <TargetDirectives>
          <RolePlayerConnectDirective>
            <AcceptingClass>
              <DomainClassMoniker Name="ModelType" />
            </AcceptingClass>
          </RolePlayerConnectDirective>
        </TargetDirectives>
      </LinkConnectDirective>
    </ConnectionBuilder>
    <ConnectionBuilder Name="EntityHasRelationsBuilder">
      <LinkConnectDirective>
        <DomainRelationshipMoniker Name="EntityHasRelations" />
        <SourceDirectives>
          <RolePlayerConnectDirective>
            <AcceptingClass>
              <DomainClassMoniker Name="ModelEntity" />
            </AcceptingClass>
          </RolePlayerConnectDirective>
        </SourceDirectives>
        <TargetDirectives>
          <RolePlayerConnectDirective>
            <AcceptingClass>
              <DomainClassMoniker Name="ModelRelation" />
            </AcceptingClass>
          </RolePlayerConnectDirective>
        </TargetDirectives>
      </LinkConnectDirective>
    </ConnectionBuilder>
    <ConnectionBuilder Name="RelationHasEntityBuilder">
      <LinkConnectDirective>
        <DomainRelationshipMoniker Name="RelationHasEntity" />
        <SourceDirectives>
          <RolePlayerConnectDirective>
            <AcceptingClass>
              <DomainClassMoniker Name="ModelRelation" />
            </AcceptingClass>
          </RolePlayerConnectDirective>
        </SourceDirectives>
        <TargetDirectives>
          <RolePlayerConnectDirective>
            <AcceptingClass>
              <DomainClassMoniker Name="ModelEntity" />
            </AcceptingClass>
          </RolePlayerConnectDirective>
        </TargetDirectives>
      </LinkConnectDirective>
    </ConnectionBuilder>
    <ConnectionBuilder Name="ModelTypeReferencesModelComplexTypesBuilder">
      <LinkConnectDirective>
        <DomainRelationshipMoniker Name="ModelTypeReferencesModelComplexTypes" />
        <SourceDirectives>
          <RolePlayerConnectDirective>
            <AcceptingClass>
              <DomainClassMoniker Name="ModelType" />
            </AcceptingClass>
          </RolePlayerConnectDirective>
        </SourceDirectives>
        <TargetDirectives>
          <RolePlayerConnectDirective>
            <AcceptingClass>
              <DomainClassMoniker Name="ModelComplexType" />
            </AcceptingClass>
          </RolePlayerConnectDirective>
        </TargetDirectives>
      </LinkConnectDirective>
    </ConnectionBuilder>
    <ConnectionBuilder Name="CaseFileEntityReferencesChildCaseFileRelationsBuilder">
      <LinkConnectDirective>
        <DomainRelationshipMoniker Name="CaseFileEntityReferencesChildCaseFileRelations" />
        <SourceDirectives>
          <RolePlayerConnectDirective>
            <AcceptingClass>
              <DomainClassMoniker Name="CaseFileEntity" />
            </AcceptingClass>
          </RolePlayerConnectDirective>
        </SourceDirectives>
        <TargetDirectives>
          <RolePlayerConnectDirective>
            <AcceptingClass>
              <DomainClassMoniker Name="CaseFileRelation" />
            </AcceptingClass>
          </RolePlayerConnectDirective>
        </TargetDirectives>
      </LinkConnectDirective>
    </ConnectionBuilder>
    <ConnectionBuilder Name="CaseFileRelationReferencesChildCaseFileEntityBuilder">
      <LinkConnectDirective>
        <DomainRelationshipMoniker Name="CaseFileRelationReferencesChildCaseFileEntity" />
        <SourceDirectives>
          <RolePlayerConnectDirective>
            <AcceptingClass>
              <DomainClassMoniker Name="CaseFileRelation" />
            </AcceptingClass>
          </RolePlayerConnectDirective>
        </SourceDirectives>
        <TargetDirectives>
          <RolePlayerConnectDirective>
            <AcceptingClass>
              <DomainClassMoniker Name="CaseFileEntity" />
            </AcceptingClass>
          </RolePlayerConnectDirective>
        </TargetDirectives>
      </LinkConnectDirective>
    </ConnectionBuilder>
    <ConnectionBuilder Name="RootCaseFileEntityBuilder">
      <LinkConnectDirective>
        <DomainRelationshipMoniker Name="RootCaseFileEntity" />
        <SourceDirectives>
          <RolePlayerConnectDirective>
            <AcceptingClass>
              <DomainClassMoniker Name="CaseFileModelSpec" />
            </AcceptingClass>
          </RolePlayerConnectDirective>
        </SourceDirectives>
        <TargetDirectives>
          <RolePlayerConnectDirective>
            <AcceptingClass>
              <DomainClassMoniker Name="CaseFileEntity" />
            </AcceptingClass>
          </RolePlayerConnectDirective>
        </TargetDirectives>
      </LinkConnectDirective>
    </ConnectionBuilder>
    <ConnectionBuilder Name="CaseFileEntityReferencesModelEntityBuilder">
      <LinkConnectDirective>
        <DomainRelationshipMoniker Name="CaseFileEntityReferencesModelEntity" />
        <SourceDirectives>
          <RolePlayerConnectDirective>
            <AcceptingClass>
              <DomainClassMoniker Name="CaseFileEntity" />
            </AcceptingClass>
          </RolePlayerConnectDirective>
        </SourceDirectives>
        <TargetDirectives>
          <RolePlayerConnectDirective>
            <AcceptingClass>
              <DomainClassMoniker Name="ModelEntity" />
            </AcceptingClass>
          </RolePlayerConnectDirective>
        </TargetDirectives>
      </LinkConnectDirective>
    </ConnectionBuilder>
    <ConnectionBuilder Name="CaseFileRelationReferencesModelRelationBuilder">
      <LinkConnectDirective>
        <DomainRelationshipMoniker Name="CaseFileRelationReferencesModelRelation" />
        <SourceDirectives>
          <RolePlayerConnectDirective>
            <AcceptingClass>
              <DomainClassMoniker Name="CaseFileRelation" />
            </AcceptingClass>
          </RolePlayerConnectDirective>
        </SourceDirectives>
        <TargetDirectives>
          <RolePlayerConnectDirective>
            <AcceptingClass>
              <DomainClassMoniker Name="ModelRelation" />
            </AcceptingClass>
          </RolePlayerConnectDirective>
        </TargetDirectives>
      </LinkConnectDirective>
    </ConnectionBuilder>
  </ConnectionBuilders>
  <Diagram Id="b3aa99e6-38af-404c-b4dc-e411ec7c8f0b" Description="" Name="ClassDiagram" DisplayName="Class Diagram" Namespace="Luminis.Its.Workbench">
    <Class>
      <DomainClassMoniker Name="ModelRoot" />
    </Class>
    <ShapeMaps>
      <CompartmentShapeMap>
        <DomainClassMoniker Name="ModelEntity" />
        <ParentElementPath>
          <DomainPath>ObjectModelSpecHasTypes.ObjectModelSpec/!ObjectModelSpec</DomainPath>
        </ParentElementPath>
        <DecoratorMap>
          <TextDecoratorMoniker Name="ModelEntityShape/Name" />
          <PropertyDisplayed>
            <PropertyPath>
              <DomainPropertyMoniker Name="NamedElement/Name" />
            </PropertyPath>
          </PropertyDisplayed>
        </DecoratorMap>
        <CompartmentShapeMoniker Name="ModelEntityShape" />
        <CompartmentMap>
          <CompartmentMoniker Name="ModelEntityShape/AttributesCompartment" />
          <ElementsDisplayed>
            <DomainPath>ModelTypeHasAttributes.Attributes/!ModelAttribute</DomainPath>
          </ElementsDisplayed>
          <PropertyDisplayed>
            <PropertyPath>
              <DomainPropertyMoniker Name="NamedElement/Name" />
            </PropertyPath>
          </PropertyDisplayed>
        </CompartmentMap>
      </CompartmentShapeMap>
      <ShapeMap>
        <DomainClassMoniker Name="Comment" />
        <ParentElementPath>
          <DomainPath>ModelRootHasComments.ModelRoot/!ModelRoot</DomainPath>
        </ParentElementPath>
        <DecoratorMap>
          <TextDecoratorMoniker Name="CommentBoxShape/Comment" />
          <PropertyDisplayed>
            <PropertyPath>
              <DomainPropertyMoniker Name="Comment/Text" />
            </PropertyPath>
          </PropertyDisplayed>
        </DecoratorMap>
        <GeometryShapeMoniker Name="CommentBoxShape" />
      </ShapeMap>
      <SwimLaneMap>
        <DomainClassMoniker Name="ObjectModelSpec" />
        <ParentElementPath>
          <DomainPath>HasObjectModelSpec.ModelRoot/!ModelRoot</DomainPath>
        </ParentElementPath>
        <DecoratorMap>
          <TextDecoratorMoniker Name="ObjectModelLane/Title" />
          <PropertyDisplayed>
            <PropertyPath>
              <DomainPropertyMoniker Name="NamedElement/Name" />
            </PropertyPath>
          </PropertyDisplayed>
        </DecoratorMap>
        <SwimLane>
          <SwimLaneMoniker Name="ObjectModelLane" />
        </SwimLane>
      </SwimLaneMap>
      <SwimLaneMap>
        <DomainClassMoniker Name="CaseFileModelSpec" />
        <ParentElementPath>
          <DomainPath>HasCaseFileModelSpec.ModelRoot/!ModelRoot</DomainPath>
        </ParentElementPath>
        <DecoratorMap>
          <TextDecoratorMoniker Name="CaseFileSpecLane/Title" />
          <PropertyDisplayed>
            <PropertyPath>
              <DomainPropertyMoniker Name="NamedElement/Name" />
            </PropertyPath>
          </PropertyDisplayed>
        </DecoratorMap>
        <SwimLane>
          <SwimLaneMoniker Name="CaseFileSpecLane" />
        </SwimLane>
      </SwimLaneMap>
      <ShapeMap>
        <DomainClassMoniker Name="CaseFileRelation" />
        <ParentElementPath>
          <DomainPath>CaseFileModelSpecHasCaseFileTypes.CaseFileModelSpec/!CaseFileModelSpec</DomainPath>
        </ParentElementPath>
        <DecoratorMap>
          <TextDecoratorMoniker Name="CaseFileRelationShape/Title" />
          <PropertyDisplayed>
            <PropertyPath>
              <DomainPropertyMoniker Name="NamedElement/Name" />
            </PropertyPath>
          </PropertyDisplayed>
        </DecoratorMap>
        <GeometryShapeMoniker Name="CaseFileRelationShape" />
      </ShapeMap>
      <ShapeMap>
        <DomainClassMoniker Name="CaseFileEntity" />
        <ParentElementPath>
          <DomainPath>CaseFileModelSpecHasCaseFileTypes.CaseFileModelSpec/!CaseFileModelSpec</DomainPath>
        </ParentElementPath>
        <DecoratorMap>
          <TextDecoratorMoniker Name="CaseFileRelationShape/Title" />
          <PropertyDisplayed>
            <PropertyPath>
              <DomainPropertyMoniker Name="NamedElement/Name" />
            </PropertyPath>
          </PropertyDisplayed>
        </DecoratorMap>
        <DecoratorMap>
          <TextDecoratorMoniker Name="CaseFileEntityShape/Title" />
          <PropertyDisplayed>
            <PropertyPath>
              <DomainPropertyMoniker Name="NamedElement/Name" />
            </PropertyPath>
          </PropertyDisplayed>
        </DecoratorMap>
        <GeometryShapeMoniker Name="CaseFileEntityShape" />
      </ShapeMap>
      <CompartmentShapeMap>
        <DomainClassMoniker Name="ModelRelation" />
        <ParentElementPath>
          <DomainPath>ObjectModelSpecHasTypes.ObjectModelSpec/!ObjectModelSpec</DomainPath>
        </ParentElementPath>
        <DecoratorMap>
          <TextDecoratorMoniker Name="ModelRelationShape/Name" />
          <PropertyDisplayed>
            <PropertyPath>
              <DomainPropertyMoniker Name="NamedElement/Name" />
            </PropertyPath>
          </PropertyDisplayed>
        </DecoratorMap>
        <CompartmentShapeMoniker Name="ModelRelationShape" />
        <CompartmentMap>
          <CompartmentMoniker Name="ModelRelationShape/AttributesCompartment" />
          <ElementsDisplayed>
            <DomainPath>ModelTypeHasAttributes.Attributes/!ModelAttribute</DomainPath>
          </ElementsDisplayed>
          <PropertyDisplayed>
            <PropertyPath>
              <DomainPropertyMoniker Name="NamedElement/Name" />
            </PropertyPath>
          </PropertyDisplayed>
        </CompartmentMap>
      </CompartmentShapeMap>
      <CompartmentShapeMap>
        <DomainClassMoniker Name="ModelComplexType" />
        <ParentElementPath>
          <DomainPath>ObjectModelSpecHasTypes.ObjectModelSpec/!ObjectModelSpec</DomainPath>
        </ParentElementPath>
        <DecoratorMap>
          <TextDecoratorMoniker Name="ModelComplexTypeShape/Name" />
          <PropertyDisplayed>
            <PropertyPath>
              <DomainPropertyMoniker Name="NamedElement/Name" />
            </PropertyPath>
          </PropertyDisplayed>
        </DecoratorMap>
        <CompartmentShapeMoniker Name="ModelComplexTypeShape" />
        <CompartmentMap>
          <CompartmentMoniker Name="ModelComplexTypeShape/AttributesCompartment" />
          <ElementsDisplayed>
            <DomainPath>ModelTypeHasAttributes.Attributes/!ModelAttribute</DomainPath>
          </ElementsDisplayed>
          <PropertyDisplayed>
            <PropertyPath>
              <DomainPropertyMoniker Name="NamedElement/Name" />
            </PropertyPath>
          </PropertyDisplayed>
        </CompartmentMap>
      </CompartmentShapeMap>
    </ShapeMaps>
    <ConnectorMaps>
      <ConnectorMap>
        <ConnectorMoniker Name="CommentConnector" />
        <DomainRelationshipMoniker Name="CommentReferencesSubjects" />
      </ConnectorMap>
      <ConnectorMap>
        <ConnectorMoniker Name="RelationConnector" />
        <DomainRelationshipMoniker Name="RelationHasEntity" />
      </ConnectorMap>
      <ConnectorMap>
        <ConnectorMoniker Name="EntityConnector" />
        <DomainRelationshipMoniker Name="EntityHasRelations" />
      </ConnectorMap>
      <ConnectorMap>
        <ConnectorMoniker Name="ComplexTypeConnector" />
        <DomainRelationshipMoniker Name="ModelTypeReferencesModelComplexTypes" />
        <DecoratorMap>
          <TextDecoratorMoniker Name="ComplexTypeConnector/Name" />
          <PropertyDisplayed>
            <PropertyPath>
              <DomainPropertyMoniker Name="ModelTypeReferencesModelComplexTypes/ContainerName" />
            </PropertyPath>
          </PropertyDisplayed>
        </DecoratorMap>
      </ConnectorMap>
      <ConnectorMap>
        <ConnectorMoniker Name="CaseFileElementConnector" />
        <DomainRelationshipMoniker Name="CaseFileEntityReferencesChildCaseFileRelations" />
      </ConnectorMap>
      <ConnectorMap>
        <ConnectorMoniker Name="CaseFileElementConnector" />
        <DomainRelationshipMoniker Name="CaseFileRelationReferencesChildCaseFileEntity" />
      </ConnectorMap>
    </ConnectorMaps>
  </Diagram>
  <Designer FileExtension="its" EditorGuid="6016cee0-5bfc-40ee-ab9c-d16c9e7a682f">
    <RootClass>
      <DomainClassMoniker Name="ModelRoot" />
    </RootClass>
    <XmlSerializationDefinition CustomPostLoad="false">
      <XmlSerializationBehaviorMoniker Name="WorkbenchSerializationBehavior" />
    </XmlSerializationDefinition>
    <ToolboxTab TabText="Class Diagrams">
      <ElementTool Name="Attribute" ToolboxIcon="resources\attributetool.bmp" Caption="Attribute" Tooltip="Create an Attribute on a Class" HelpKeyword="AttributeF1Keyword">
        <DomainClassMoniker Name="ModelAttribute" />
      </ElementTool>
      <ElementTool Name="Comment" ToolboxIcon="resources\commenttool.bmp" Caption="Comment" Tooltip="Create a Comment" HelpKeyword="CommentF1Keyword">
        <DomainClassMoniker Name="Comment" />
      </ElementTool>
      <ConnectionTool Name="CommentsReferenceTypes" ToolboxIcon="resources\commentlinktool.bmp" Caption="Comment Link" Tooltip="Link a comment to an element" HelpKeyword="CommentsReferenceTypesF1Keyword">
        <ConnectionBuilderMoniker Name="Workbench/CommentReferencesSubjectsBuilder" />
      </ConnectionTool>
      <ElementTool Name="ModelRelation" ToolboxIcon="Resources\ClassTool.bmp" Caption="ModelRelation" Tooltip="Model Relation" HelpKeyword="ModelRelation">
        <DomainClassMoniker Name="ModelRelation" />
      </ElementTool>
      <ElementTool Name="ModelComplexType" ToolboxIcon="Resources\ClassTool.bmp" Caption="ModelComplexType" Tooltip="Model Complex Type" HelpKeyword="ModelComplexType">
        <DomainClassMoniker Name="ModelComplexType" />
      </ElementTool>
      <ConnectionTool Name="ModelComplexTypeConnector" ToolboxIcon="Resources\AssociationTool.bmp" Caption="ModelComplexTypeConnector" Tooltip="Model Complex Type Connector" HelpKeyword="ModelComplexTypeConnector">
        <ConnectionBuilderMoniker Name="Workbench/ModelTypeReferencesModelComplexTypesBuilder" />
      </ConnectionTool>
      <ElementTool Name="ModelEntityTool" ToolboxIcon="Resources\ClassTool.bmp" Caption="ModelEntity" Tooltip="Model Entity Tool" HelpKeyword="ModelEntityTool">
        <DomainClassMoniker Name="ModelEntity" />
      </ElementTool>
      <ConnectionTool Name="EntityRelationConnector" ToolboxIcon="Resources\UnidirectionTool.bmp" Caption="EntityRelationConnector" Tooltip="Entity Relation Connector" HelpKeyword="EntityRelationConnector">
        <ConnectionBuilderMoniker Name="Workbench/RelationHasEntityBuilder" />
      </ConnectionTool>
      <ConnectionTool Name="RelationEntityConnector" ToolboxIcon="Resources\UnidirectionTool.bmp" Caption="RelationEntityConnector" Tooltip="Relation Entity Connector" HelpKeyword="RelationEntityConnector">
        <ConnectionBuilderMoniker Name="Workbench/EntityHasRelationsBuilder" />
      </ConnectionTool>
    </ToolboxTab>
    <ToolboxTab TabText="Workbench">
      <ElementTool Name="CaseFileEntityTool" ToolboxIcon="Resources\ClassTool.bmp" Caption="CaseFileEntity" Tooltip="Case File Entity Tool" HelpKeyword="CaseFileEntityTool">
        <DomainClassMoniker Name="CaseFileEntity" />
      </ElementTool>
      <ElementTool Name="CaseFileRelationTool" ToolboxIcon="Resources\ClassTool.bmp" Caption="CaseFileRelation" Tooltip="Case File Relation Tool" HelpKeyword="CaseFileRelationTool">
        <DomainClassMoniker Name="CaseFileRelation" />
      </ElementTool>
    </ToolboxTab>
    <Validation UsesMenu="false" UsesOpen="false" UsesSave="false" UsesLoad="false" />
    <DiagramMoniker Name="ClassDiagram" />
  </Designer>
  <Explorer ExplorerGuid="4e973415-dda5-445b-a27f-0b624f7d6cb0" Title="">
    <ExplorerBehaviorMoniker Name="Workbench/WorkbenchExplorer" />
  </Explorer>
</Dsl>