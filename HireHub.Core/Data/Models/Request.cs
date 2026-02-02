using HireHub.Shared.Common.Models;

namespace HireHub.Core.Data.Models;

using System;

public class Request : BaseEntity
{
    public Request() : base("requests")
    {
    }

    public int RequestId { get; set; }
    public RequestType RequestType { get; set; }
    public RequestSubType SubType { get; set; }
    public RequestStatus Status { get; set; } = RequestStatus.Pending;
    public int? ApprovedBy { get; set; }
    public DateTime? ApprovedDate { get; set; }
    public int RequestedBy { get; set; }
    public DateTime RequestedDate { get; set; } = DateTime.Now;

    // Navigation
    public User? Approver { get; set; }
    public User? Requester { get; set; }
}

public enum RequestType
{
    AddAction
}

public enum RequestSubType
{
    ViewDrive,
    AddDrive,
    UpdateDrive,
    DeleteDrive
}

public enum RequestStatus
{
    Pending,
    Approved,
    Rejected
}


