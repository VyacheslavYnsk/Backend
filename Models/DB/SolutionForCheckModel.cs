using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Domain.Abstractions;
using Domain.Entities;

namespace Api.Models
{
    public class SolutionForCheckModel : Entity//2 fk         навигации - 2
    {


        // [Required]
        // public CriteriaAssignments CriteriaAssegment { get; set; }

        public Guid SolutionId { get; set; }

        public string Comment { get; set; }
         
        public Guid AuthortId { get; set; }

        public DateTime DueTime { get; set; }

        public bool IsChecked { get; set; } = false;
    }
}