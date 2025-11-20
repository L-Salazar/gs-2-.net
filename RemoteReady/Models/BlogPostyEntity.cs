using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RemoteReady.Models
{
    [Table("TB_GS_NET_BLOG_POST")]
    public class BlogPostEntity
    {
        [Key]
        [Column("ID_POST")]
        public int Id { get; set; }

        [Required(ErrorMessage = "Campo título é obrigatório")]
        [StringLength(120, ErrorMessage = "Campo não pode ter mais que 120 caracteres")]
        [Column("DS_TITULO")]
        public string Titulo { get; set; } = string.Empty;

        [StringLength(600, ErrorMessage = "Campo não pode ter mais que 600 caracteres")]
        [Column("DS_DESCRICAO")]
        public string? Descricao { get; set; }

        [StringLength(300, ErrorMessage = "Campo não pode ter mais que 300 caracteres")]
        [Column("DS_IMAGE_URL")]
        public string? ImageUrl { get; set; }

        [StringLength(50, ErrorMessage = "Campo não pode ter mais que 50 caracteres")]
        [Column("DS_TAG")]
        public string? Tag { get; set; }

        [Column("DT_CRIACAO")]
        public DateTime DataCriacao { get; set; } = DateTime.Now;
    }
}