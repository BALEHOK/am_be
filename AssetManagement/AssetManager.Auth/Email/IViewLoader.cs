﻿namespace AssetManager.Auth.Email
{
    public interface IViewLoader
    {
        string RenderViewToString(string viewPath, object model = null);
    }
}