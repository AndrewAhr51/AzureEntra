using Microsoft.SharePoint.Client;
using EntraIdBL.Models;

namespace NeudesicConsole.SharePointHelper;

public class SharePointListConnector
{
    private string _siteUrl;
    private string _listName;
    private ClientContext _context;
    private List _list;

    public SharePointListConnector(string siteUrl, string listName)
    {
        _siteUrl = siteUrl;
        _listName = listName;
    }

    public void Connect()
    {
        _context = new ClientContext(_siteUrl);
        _list = _context.Web.Lists.GetByTitle(_listName);
        _context.Load(_list);
        _context.ExecuteQuery();
    }

    public void DeleteAllItems()
    {
        CamlQuery query = CamlQuery.CreateAllItemsQuery();
        ListItemCollection items = _list.GetItems(query);
        _context.Load(items);
        _context.ExecuteQuery();

        foreach (ListItem item in items)
        {
            item.DeleteObject();
        }

        _context.ExecuteQuery();
    }

    public void AddItem(EntraIdRecord record)
    {
        ListItemCreationInformation itemCreateInfo = new ListItemCreationInformation();
        ListItem newItem = _list.AddItem(itemCreateInfo);

        newItem["Title"] = record.UserPrincipalName;
        newItem["DisplayName"] = record.DisplayName;
        newItem["JobTitle"] = record.JobTitle;
        newItem["OfficeLocation"] = record.OfficeLocation;
        newItem["Department"] = record.Department;
        newItem["EmailAddress"] = record.EmailAddress;
        newItem["Manager"] = record.Manager;

        newItem.Update();
        _context.ExecuteQuery();
    }
}
