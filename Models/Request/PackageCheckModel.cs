using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Domain.Abstractions;
using Domain.Entities;

namespace Api.Models
{
    public class PackageCheckModel 
    {

        public List<SolutionForCheckModel> SolutionForCheckTasks { get; set; }

        public string Deadline { get; set; }



    }
}