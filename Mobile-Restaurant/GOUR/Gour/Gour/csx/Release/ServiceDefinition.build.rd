<?xml version="1.0" encoding="utf-8"?>
<serviceModel xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema" name="Gour" generation="1" functional="0" release="0" Id="8a93a995-a83c-4d88-aa55-61187eb5a751" dslVersion="1.2.0.0" xmlns="http://schemas.microsoft.com/dsltools/RDSM">
  <groups>
    <group name="GourGroup" generation="1" functional="0" release="0">
      <componentports>
        <inPort name="Gour.Web:Endpoint1" protocol="https">
          <inToChannel>
            <lBChannelMoniker name="/Gour/GourGroup/LB:Gour.Web:Endpoint1" />
          </inToChannel>
        </inPort>
        <inPort name="Gour.Web:Endpoint2" protocol="http">
          <inToChannel>
            <lBChannelMoniker name="/Gour/GourGroup/LB:Gour.Web:Endpoint2" />
          </inToChannel>
        </inPort>
      </componentports>
      <settings>
        <aCS name="Certificate|Gour.Web:SslCertificate" defaultValue="">
          <maps>
            <mapMoniker name="/Gour/GourGroup/MapCertificate|Gour.Web:SslCertificate" />
          </maps>
        </aCS>
        <aCS name="Gour.Web:BlobSASExperiationTime" defaultValue="">
          <maps>
            <mapMoniker name="/Gour/GourGroup/MapGour.Web:BlobSASExperiationTime" />
          </maps>
        </aCS>
        <aCS name="Gour.Web:ContainerSASExperiationTime" defaultValue="">
          <maps>
            <mapMoniker name="/Gour/GourGroup/MapGour.Web:ContainerSASExperiationTime" />
          </maps>
        </aCS>
        <aCS name="Gour.Web:SqlSampleDataContextConnectionString" defaultValue="">
          <maps>
            <mapMoniker name="/Gour/GourGroup/MapGour.Web:SqlSampleDataContextConnectionString" />
          </maps>
        </aCS>
        <aCS name="Gour.WebInstances" defaultValue="[1,1,1]">
          <maps>
            <mapMoniker name="/Gour/GourGroup/MapGour.WebInstances" />
          </maps>
        </aCS>
      </settings>
      <channels>
        <lBChannel name="LB:Gour.Web:Endpoint1">
          <toPorts>
            <inPortMoniker name="/Gour/GourGroup/Gour.Web/Endpoint1" />
          </toPorts>
        </lBChannel>
        <lBChannel name="LB:Gour.Web:Endpoint2">
          <toPorts>
            <inPortMoniker name="/Gour/GourGroup/Gour.Web/Endpoint2" />
          </toPorts>
        </lBChannel>
      </channels>
      <maps>
        <map name="MapCertificate|Gour.Web:SslCertificate" kind="Identity">
          <certificate>
            <certificateMoniker name="/Gour/GourGroup/Gour.Web/SslCertificate" />
          </certificate>
        </map>
        <map name="MapGour.Web:BlobSASExperiationTime" kind="Identity">
          <setting>
            <aCSMoniker name="/Gour/GourGroup/Gour.Web/BlobSASExperiationTime" />
          </setting>
        </map>
        <map name="MapGour.Web:ContainerSASExperiationTime" kind="Identity">
          <setting>
            <aCSMoniker name="/Gour/GourGroup/Gour.Web/ContainerSASExperiationTime" />
          </setting>
        </map>
        <map name="MapGour.Web:SqlSampleDataContextConnectionString" kind="Identity">
          <setting>
            <aCSMoniker name="/Gour/GourGroup/Gour.Web/SqlSampleDataContextConnectionString" />
          </setting>
        </map>
        <map name="MapGour.WebInstances" kind="Identity">
          <setting>
            <sCSPolicyIDMoniker name="/Gour/GourGroup/Gour.WebInstances" />
          </setting>
        </map>
      </maps>
      <components>
        <groupHascomponents>
          <role name="Gour.Web" generation="1" functional="0" release="0" software="c:\users\nick\documents\visual studio 2010\Projects\Gour\Gour\Gour\csx\Release\roles\Gour.Web" entryPoint="base\x64\WaHostBootstrapper.exe" parameters="base\x64\WaIISHost.exe " memIndex="768" hostingEnvironment="frontendadmin" hostingEnvironmentVersion="2">
            <componentports>
              <inPort name="Endpoint1" protocol="https" portRanges="443">
                <certificate>
                  <certificateMoniker name="/Gour/GourGroup/Gour.Web/SslCertificate" />
                </certificate>
              </inPort>
              <inPort name="Endpoint2" protocol="http" portRanges="10080" />
            </componentports>
            <settings>
              <aCS name="BlobSASExperiationTime" defaultValue="" />
              <aCS name="ContainerSASExperiationTime" defaultValue="" />
              <aCS name="SqlSampleDataContextConnectionString" defaultValue="" />
              <aCS name="__ModelData" defaultValue="&lt;m role=&quot;Gour.Web&quot; xmlns=&quot;urn:azure:m:v1&quot;&gt;&lt;r name=&quot;Gour.Web&quot;&gt;&lt;e name=&quot;Endpoint1&quot; /&gt;&lt;e name=&quot;Endpoint2&quot; /&gt;&lt;/r&gt;&lt;/m&gt;" />
            </settings>
            <resourcereferences>
              <resourceReference name="DiagnosticStore" defaultAmount="[4096,4096,4096]" defaultSticky="true" kind="Directory" />
              <resourceReference name="EventStore" defaultAmount="[1000,1000,1000]" defaultSticky="false" kind="LogStore" />
            </resourcereferences>
            <storedcertificates>
              <storedCertificate name="Stored0SslCertificate" certificateStore="My" certificateLocation="System">
                <certificate>
                  <certificateMoniker name="/Gour/GourGroup/Gour.Web/SslCertificate" />
                </certificate>
              </storedCertificate>
            </storedcertificates>
            <certificates>
              <certificate name="SslCertificate" />
            </certificates>
          </role>
          <sCSPolicy>
            <sCSPolicyIDMoniker name="/Gour/GourGroup/Gour.WebInstances" />
            <sCSPolicyFaultDomainMoniker name="/Gour/GourGroup/Gour.WebFaultDomains" />
          </sCSPolicy>
        </groupHascomponents>
      </components>
      <sCSPolicy>
        <sCSPolicyFaultDomain name="Gour.WebFaultDomains" defaultPolicy="[2,2,2]" />
        <sCSPolicyID name="Gour.WebInstances" defaultPolicy="[1,1,1]" />
      </sCSPolicy>
    </group>
  </groups>
  <implements>
    <implementation Id="16d16dd5-168f-4803-bf8b-58cd20ee6198" ref="Microsoft.RedDog.Contract\ServiceContract\GourContract@ServiceDefinition.build">
      <interfacereferences>
        <interfaceReference Id="11d88940-6d7f-4ef2-a28f-7621d53d1fd0" ref="Microsoft.RedDog.Contract\Interface\Gour.Web:Endpoint1@ServiceDefinition.build">
          <inPort>
            <inPortMoniker name="/Gour/GourGroup/Gour.Web:Endpoint1" />
          </inPort>
        </interfaceReference>
        <interfaceReference Id="650d661c-a996-4435-810e-6a00e43acf1a" ref="Microsoft.RedDog.Contract\Interface\Gour.Web:Endpoint2@ServiceDefinition.build">
          <inPort>
            <inPortMoniker name="/Gour/GourGroup/Gour.Web:Endpoint2" />
          </inPort>
        </interfaceReference>
      </interfacereferences>
    </implementation>
  </implements>
</serviceModel>