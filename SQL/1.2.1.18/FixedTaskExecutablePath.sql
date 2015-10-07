update Task set ExecutablePath='~/'+REPLACE(SUBSTRING(ExecutablePath,CHARINDEX('App_Data',ExecutablePath),
LEN(ExecutablePath)-CHARINDEX('App_Data',ExecutablePath)+LEN('App_Data')),'\','/') 
where ExecutablePath is not null  and CHARINDEX('App_Data',ExecutablePath)>0