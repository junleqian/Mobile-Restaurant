<?xml version="1.0" encoding="utf-8"?>
<serviceModel xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema" name="Microsoft.Samples.CRUDSqlAzure" generation="1" functional="0" release="0" Id="f4338e53-54c4-456f-b448-1f5f74e07540" dslVersion="1.2.0.0" xmlns="http://schemas.microsoft.com/dsltools/RDSM">
  <groups>
    <group name="Microsoft.Samples.CRUDSqlAzureGroup" generation="1" functional="0" release="0">
      <componentports>
        <inPort name="CRUDSqlAzure.Web:Endpoint1" protocol="https">
          <inToChannel>
            <lBChannelMoniker name="/Microsoft.Samples.CRUDSqlAzure/Microsoft.Samples.CRUDSqlAzureGroup/LB:CRUDSqlAzure.Web:Endpoint1" />
          </inToChannel>
        </inPort>
        <inPort name="CRUDSqlAzure.Web:Endpoint2" protocol="http">
          <inToChannel>
            <lBChannelMoniker name="/Microsoft.Samples.CRUDSqlAzure/Microsoft.Samples.CRUDSqlAzureGroup/LB:CRUDSqlAzure.Web:Endpoint2" />
          </inToChannel>
        </inPort>
      </componentports>
      <settings>
        <aCS name="Certificate|CRUDSqlAzure.Web:SslCertificate" defaultValue="">
          <maps>
            <mapMoniker name="/Microsoft.Samples.CRUDSqlAzure/Microsoft.Samples.CRUDSqlAzureGroup/MapCertificate|CRUDSqlAzure.Web:SslCertificate" />
          </maps>
        </aCS>
        <aCS name="CRUDSqlAzure.Web:BlobSASExperiationTime" defaultValue="">
          <maps>
            <mapMoniker name="/Microsoft.Samples.CRUDSqlAzure/Microsoft.Samples.CRUDSqlAzureGroup/MapCRUDSqlAzure.Web:BlobSASExperiationTime" />
          </maps>
        </aCS>
        <aCS name="CRUDSqlAzure.Web:ContainerSASExperiationTime" defaultValue="">
          <maps>
            <mapMoniker name="/Microsoft.Samples.CRUDSqlAzure/Microsoft.Samples.CRUDSqlAzureGroup/MapCRUDSqlAzure.Web:ContainerSASExperiationTime" />
          </maps>
        </aCS>
        <aCS name="CRUDSqlAzure.Web:realm" defaultValue="">
          <maps>
            <mapMoniker name="/Microsoft.Samples.CRUDSqlAzure/Microsoft.Samples.CRUDSqlAzureGroup/MapCRUDSqlAzure.Web:realm" />
          </maps>
        </aCS>
        <aCS name="CRUDSqlAzure.Web:serviceKey" defaultValue="">
          <maps>
            <mapMoniker name="/Microsoft.Samples.CRUDSqlAzure/Microsoft.Samples.CRUDSqlAzureGroup/MapCRUDSqlAzure.Web:serviceKey" />
          </maps>
        </aCS>
        <aCS name="CRUDSqlAzure.Web:SqlSampleDataContextConnectionString" defaultValue="">
          <maps>
            <mapMoniker name="/Microsoft.Samples.CRUDSqlAzure/Microsoft.Samples.CRUDSqlAzureGroup/MapCRUDSqlAzure.Web:SqlSampleDataContextConnectionString" />
          </maps>
        </aCS>
        <aCS name="CRUDSqlAzure.Web:trustedIssuerName" defaultValue="">
          <maps>
            <mapMoniker name="/Microsoft.Samples.CRUDSqlAzure/Microsoft.Samples.CRUDSqlAzureGroup/MapCRUDSqlAzure.Web:trustedIssuerName" />
          </maps>
        </aCS>
        <aCS name="CRUDSqlAzure.Web:trustedIssuersIdentifier" defaultValue="">
          <maps>
            <mapMoniker name="/Microsoft.Samples.CRUDSqlAzure/Microsoft.Samples.CRUDSqlAzureGroup/MapCRUDSqlAzure.Web:trustedIssuersIdentifier" />
          </maps>
        </aCS>
        <aCS name="CRUDSqlAzure.WebInstances" defaultValue="[1,1,1]">
          <maps>
            <mapMoniker name="/Microsoft.Samples.CRUDSqlAzure/Microsoft.Samples.CRUDSqlAzureGroup/MapCRUDSqlAzure.WebInstances" />
          </maps>
        </aCS>
      </settings>
      <channels>
        <lBChannel name="LB:CRUDSqlAzure.Web:Endpoint1">
          <toPorts>
            <inPortMoniker name="/Microsoft.Samples.CRUDSqlAzure/Microsoft.Samples.CRUDSqlAzureGroup/CRUDSqlAzure.Web/Endpoint1" />
          </toPorts>
        </lBChannel>
        <lBChannel name="LB:CRUDSqlAzure.Web:Endpoint2">
          <toPorts>
            <inPortMoniker name="/Microsoft.Samples.CRUDSqlAzure/Microsoft.Samples.CRUDSqlAzureGroup/CRUDSqlAzure.Web/Endpoint2" />
          </toPorts>
        </lBChannel>
      </channels>
      <maps>
        <map name="MapCertificate|CRUDSqlAzure.Web:SslCertificate" kind="Identity">
          <certificate>
            <certificateMoniker name="/Microsoft.Samples.CRUDSqlAzure/Microsoft.Samples.CRUDSqlAzureGroup/CRUDSqlAzure.Web/SslCertificate" />
          </certificate>
        </map>
        <map name="MapCRUDSqlAzure.Web:BlobSASExperiationTime" kind="Identity">
          <setting>
            <aCSMoniker name="/Microsoft.Samples.CRUDSqlAzure/Microsoft.Samples.CRUDSqlAzureGroup/CRUDSqlAzure.Web/BlobSASExperiationTime" />
          </setting>
        </map>
        <map name="MapCRUDSqlAzure.Web:ContainerSASExperiationTime" kind="Identity">
          <setting>
            <aCSMoniker name="/Microsoft.Samples.CRUDSqlAzure/Microsoft.Samples.CRUDSqlAzureGroup/CRUDSqlAzure.Web/ContainerSASExperiationTime" />
          </setting>
        </map>
        <map name="MapCRUDSqlAzure.Web:realm" kind="Identity">
          <setting>
            <aCSMoniker name="/Microsoft.Samples.CRUDSqlAzure/Microsoft.Samples.CRUDSqlAzureGroup/CRUDSqlAzure.Web/realm" />
          </setting>
        </map>
        <map name="MapCRUDSqlAzure.Web:serviceKey" kind="Identity">
          <setting>
            <aCSMoniker name="/Microsoft.Samples.CRUDSqlAzure/Microsoft.Samples.CRUDSqlAzureGroup/CRUDSqlAzure.Web/serviceKey" />
          </setting>
        </map>
        <map name="MapCRUDSqlAzure.Web:SqlSampleDataContextConnectionString" kind="Identity">
          <setting>
            <aCSMoniker name="/Microsoft.Samples.CRUDSqlAzure/Microsoft.Samples.CRUDSqlAzureGroup/CRUDSqlAzure.Web/SqlSampleDataContextConnectionString" />
          </setting>
        </map>
        <map name="MapCRUDSqlAzure.Web:trustedIssuerName" kind="Identity">
          <setting>
            <aCSMoniker name="/Microsoft.Samples.CRUDSqlAzure/Microsoft.Samples.CRUDSqlAzureGroup/CRUDSqlAzure.Web/trustedIssuerName" />
          </setting>
        </map>
        <map name="MapCRUDSqlAzure.Web:trustedIssuersIdentifier" kind="Identity">
          <setting>
            <aCSMoniker name="/Microsoft.Samples.CRUDSqlAzure/Microsoft.Samples.CRUDSqlAzureGroup/CRUDSqlAzure.Web/trustedIssuersIdentifier" />
          </setting>
        </map>
        <map name="MapCRUDSqlAzure.WebInstances" kind="Identity">
          <setting>
            <sCSPolicyIDMoniker name="/Microsoft.Samples.CRUDSqlAzure/Microsoft.Samples.CRUDSqlAzureGroup/CRUDSqlAzure.WebInstances" />
          </setting>
        </map>
      </maps>
      <components>
        <groupHascomponents>
          <role name="CRUDSqlAzure.Web" generation="1" functional="0" release="0" software="C:\WindowsAzure\WATWindowsPhone2\Samples\WP7.1\CRUDSqlAzure3\CRUDSqlAzure\csx\Debug\roles\CRUDSqlAzure.Web" entryPoint="base\x64\WaHostBootstrapper.exe" parameters="base\x64\WaIISHost.exe " memIndex="768" hostingEnvironment="frontendadmin" hostingEnvironmentVersion="2">
            <componentports>
              <inPort name="Endpoint1" protocol="https" portRanges="443">
                <certificate>
                  <certificateMoniker name="/Microsoft.Samples.CRUDSqlAzure/Microsoft.Samples.CRUDSqlAzureGroup/CRUDSqlAzure.Web/SslCertificate" />
                </certificate>
              </inPort>
              <inPort name="Endpoint2" protocol="http" portRanges="10080" />
            </componentports>
            <settings>
              <aCS name="BlobSASExperiationTime" defaultValue="" />
              <aCS name="ContainerSASExperiationTime" defaultValue="" />
              <aCS name="realm" defaultValue="" />
              <aCS name="serviceKey" defaultValue="" />
              <aCS name="SqlSampleDataContextConnectionString" defaultValue="" />
              <aCS name="trustedIssuerName" defaultValue="" />
              <aCS name="trustedIssuersIdentifier" defaultValue="" />
              <aCS name="__ModelData" defaultValue="&lt;m role=&quot;CRUDSqlAzure.Web&quot; xmlns=&quot;urn:azure:m:v1&quot;&gt;&lt;r name=&quot;CRUDSqlAzure.Web&quot;&gt;&lt;e name=&quot;Endpoint1&quot; /&gt;&lt;e name=&quot;Endpoint2&quot; /&gt;&lt;/r&gt;&lt;/m&gt;" />
            </settings>
            <resourcereferences>
              <resourceReference name="DiagnosticStore" defaultAmount="[4096,4096,4096]" defaultSticky="true" kind="Directory" />
              <resourceReference name="EventStore" defaultAmount="[1000,1000,1000]" defaultSticky="false" kind="LogStore" />
            </resourcereferences>
            <storedcertificates>
              <storedCertificate name="Stored0SslCertificate" certificateStore="My" certificateLocation="System">
                <certificate>
                  <certificateMoniker name="/Microsoft.Samples.CRUDSqlAzure/Microsoft.Samples.CRUDSqlAzureGroup/CRUDSqlAzure.Web/SslCertificate" />
                </certificate>
              </storedCertificate>
            </storedcertificates>
            <certificates>
              <certificate name="SslCertificate" />
            </certificates>
          </role>
          <sCSPolicy>
            <sCSPolicyIDMoniker name="/Microsoft.Samples.CRUDSqlAzure/Microsoft.Samples.CRUDSqlAzureGroup/CRUDSqlAzure.WebInstances" />
            <sCSPolicyFaultDomainMoniker name="/Microsoft.Samples.CRUDSqlAzure/Microsoft.Samples.CRUDSqlAzureGroup/CRUDSqlAzure.WebFaultDomains" />
          </sCSPolicy>
        </groupHascomponents>
      </components>
      <sCSPolicy>
        <sCSPolicyFaultDomain name="CRUDSqlAzure.WebFaultDomains" defaultPolicy="[2,2,2]" />
        <sCSPolicyID name="CRUDSqlAzure.WebInstances" defaultPolicy="[1,1,1]" />
      </sCSPolicy>
    </group>
  </groups>
  <implements>
    <implementation Id="dae5be4e-e6a7-4e45-a290-8fd09a66f9ea" ref="Microsoft.RedDog.Contract\ServiceContract\Microsoft.Samples.CRUDSqlAzureContract@ServiceDefinition.build">
      <interfacereferences>
        <interfaceReference Id="df15e745-dc5d-45b0-ae55-04b012504455" ref="Microsoft.RedDog.Contract\Interface\CRUDSqlAzure.Web:Endpoint1@ServiceDefinition.build">
          <inPort>
            <inPortMoniker name="/Microsoft.Samples.CRUDSqlAzure/Microsoft.Samples.CRUDSqlAzureGroup/CRUDSqlAzure.Web:Endpoint1" />
          </inPort>
        </interfaceReference>
        <interfaceReference Id="bdfb3bd0-52b3-4109-b6dd-2ac498b8f3d2" ref="Microsoft.RedDog.Contract\Interface\CRUDSqlAzure.Web:Endpoint2@ServiceDefinition.build">
          <inPort>
            <inPortMoniker name="/Microsoft.Samples.CRUDSqlAzure/Microsoft.Samples.CRUDSqlAzureGroup/CRUDSqlAzure.Web:Endpoint2" />
          </inPort>
        </interfaceReference>
      </interfacereferences>
    </implementation>
  </implements>
</serviceModel>