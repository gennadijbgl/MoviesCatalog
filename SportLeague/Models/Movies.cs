namespace SportLeague.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Movies
    {
        public int Id { get; set; }

        [DisplayName("Posted by")]
        public int IdUser { get; set; }

        [Required]
        [StringLength(50)]
        public string Title { get; set; }

        [Required]
        [StringLength(550)]

        public string Description { get; set; }

        [Range(1920, 2017,ErrorMessage = "The year must be in range 1920, 2017")]
        public int Year { get; set; }

        [StringLength(50)]
        public string Director { get; set; }

        [StringLength(250)]
        [DisplayName("Poster")]
        public string PosterURL { get; set; }

        public virtual Users Users { get; set; }
    }
}
