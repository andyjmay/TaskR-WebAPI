using System;

namespace TaskR.Models {
  public class Task {
    public int TaskID { get; set; }
    public string Title { get; set; }
    public string Details { get; set; }
    public string AssignedTo { get; set; }
    public string Status { get; set; }
    public DateTime DateCreated { get; set; }
    public bool IsDeleted { get; set; }
  }
}