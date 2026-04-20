using System.ComponentModel.DataAnnotations;

namespace ProjectApprovalSystem.Models.ViewModels
{
    public class ManageResearchAreaViewModel
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        [Display(Name = "Research Area Name")]
        public string Name { get; set; } = string.Empty;

        [StringLength(500)]
        [Display(Name = "Description")]
        public string Description { get; set; } = string.Empty;
    }
}