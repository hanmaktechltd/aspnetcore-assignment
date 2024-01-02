using Microsoft.AspNetCore.Mvc;
using Queue_Management_System.Models;

public class CheckinViewModel
{

    public int SelectedServicePointId {get; set;}

    public int CurrentServiceProviderId {get; set;}

    public List<ServicePoint> CurrentServiceProviderServicePoints {get; set;}

}