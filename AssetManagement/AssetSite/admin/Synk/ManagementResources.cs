using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Management;
using AssetSite.admin.Synk;
using System.Configuration;
using AppFramework.ConstantsEnumerators;


/// <summary>
/// class for researching net resources
/// </summary>


public class ManagementResources
{
    public ManagementResources ()
    {
    }

    /// <summary>
    /// method  giving net directories back to the server
    /// </summary>
    /// <returns></returns>
    public List<string> GetSharedDirectories()
    {
        List<string> sharedFolders = new List<string>();

        var synkFolderPath = ConfigurationManager.AppSettings[Constants.SynkFolderPath];
        sharedFolders.Add(synkFolderPath);

        return sharedFolders;
    }
}






       














    
    

    
    

       
    



    

    
