'CRUDSqlAzure' Sample Setup
===========================

Before running the sample, you need to:
• Setup a connection string to SQL Server or SQL Azure database
• Setup an ACS namespace for it and configure the solution files

To achieve this, you can run the SetupSample.cmd script located in the root folder of the sample solution. The script will configure the database connection string and the ACS settings in the configuration files with the corresponding values for you.

The Sample setup utility will prompt for:
• The Database server to use (SQL Server or SQL Azure) and its required configuration settings needed to build a connection string
• The ACS namespace and its Management Service Key. The Management Service Key can be obtained from the Management service screen of your ACS namespace (https://{yournamespace}.accesscontrol.windows.net)
