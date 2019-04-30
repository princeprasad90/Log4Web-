# Log4Web
Logger that can add logs to db as well as in the file with the help of SP

```
 <configSections>
    <sectionGroup name="Log4Web">
      <section name="GlobalSettings" type="System.Configuration.NameValueSectionHandler" />
      <section name="SP_PROCS_FIELDS" type="System.Configuration.NameValueSectionHandler" />
    </sectionGroup>
  </configSections>
  <Log4Web>
    <GlobalSettings>
      <add key="DatabseConnection" value="LOGConString" />
      <add key="Filepath" value="D:\Logs\" />
      <add key="Filename" value="LOG.log" />
      <add key="FileDeleteDays" value="-6" />
      <add key="FileNameWithDate" value="_dd_MM_yyyy" />
      <add key="SP_NAME" value="APILOGS" />
      <add key="SP_STATUS1" value="1" />
      <add key="SP_STATUS2" value="1" />
    </GlobalSettings>
    <SP_PROCS_FIELDS>
      <add key="SP_STATUS1" value="STATUS" />
      <add key="SP_STATUS2" value="SUBSTATUS" />
      <add key="MODULE" value="VAR_1" />
      <add key="URL" value="VAR_2" />
      <add key="STATUS" value="VAR_3" />
      <add key="REQUEST" value="VARMAX_1" />
      <add key="RESPONSE" value="VARMAX_2" />
      <add key="ERRPRMSG" value="VARMAX_3" />
      <add key="REMARKS" value="VARMAX_4" />
      <add key="MISC1" value="" />
      <add key="MISC2" value="" />
      <add key="MISC3" value="" />
      <add key="MISC_INT1" value="INT_1" />
      <add key="MISC_INT2" value="" />
    </SP_PROCS_FIELDS>
  </Log4Web>
```
```
  Log4Web LogFile1 = new Log4Web(new SettingsModel() { Filename = "Log.txt" });
    #region Logger
                    DataModel except = new DataModel()
                    {
                        MODULE = "Employee",
                        REQUEST = JsonConvert.SerializeObject(Employee),
                        URL = "Employee",
                        STATUS = "7",
                        MISC_INT1 = 7,
                    };
              LogFile1.Log_DB(except);
     #endregion
```
