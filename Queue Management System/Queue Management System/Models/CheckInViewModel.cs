using Microsoft.AspNetCore.Mvc;
using Queue_Management_System.Models;

public class CheckinViewModel
{

    public int SelectedServicePointId {get; set;}
    
    public List<ServicePoint> ServicePoints {get; set;}


    //public string ServicePointName { get; set; }


}