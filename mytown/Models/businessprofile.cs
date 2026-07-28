using MyTown.Models;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace mytown.Models
{
    [Table("business_profiles")]
    public class BusinessProfile
    {
        [Key]
        [Column("business_profile_id")]
        [JsonPropertyName("businessprofile_id")]
        public int BusinessProfileId { get; set; }

        [ForeignKey(nameof(BusinessRegister))]
        [Column("bus_reg_id")]
        [JsonPropertyName("bus_reg_id")]
        public int BusRegId { get; set; }

        [Column("business_name")]
        [StringLength(100)]
        [JsonPropertyName("business_name")]
        public string? BusinessName { get; set; }

        //[Column("business_tagline")]
        //[StringLength(150)]
        //[JsonPropertyName("business_tagline")]
        //public string? BusinessTagline { get; set; }

        [Column("business_location")]
        [StringLength(255)]
        [JsonPropertyName("business_location")]
        public string? BusinessLocation { get; set; }

        [Column("business_about")]
        [StringLength(500)]
        [JsonPropertyName("business_about")]
        public string? BusinessAbout { get; set; }

        [Column("banner_path")]
        [StringLength(255)]
        [JsonPropertyName("banner_path")]
        public string? BannerPath { get; set; }

        [Column("logo_path")]
        [StringLength(255)]
        [JsonPropertyName("logo_path")]
        public string? LogoPath { get; set; }

        [Column("profile_status")]
        [StringLength(50)]
        [JsonPropertyName("profile_status")]
        public string? ProfileStatus { get; set; }

        [Column("bus_cat_id")]
        [JsonPropertyName("bus_cat_id")]
        public int BusCatId { get; set; }

        [Column("bus_serv_id")]
        [JsonPropertyName("bus_serv_id")]
        public int BusServId { get; set; }

        [Column("approved_date")]
        [JsonPropertyName("approved_date")]
        public DateTime? ApprovedDate { get; set; }

        [JsonIgnore]
        public virtual BusinessRegister? BusinessRegister { get; set; }
    }
}
