using System;
using System.Web.UI;
public interface ICommnadInterFace
{
    string SaveToolTip { get;set; }
    string SaveText { get;set; }
    string SaveCommandName { get;set; }
    string SaveCommandArgument { get;set; }
    bool SaveIsEnabled { get;set; }
}