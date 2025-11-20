using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RemoteReady.Models
{
    [Table("TB_GS_NET_EMPRESA")]
    public class EmpresaEntity
    {
        [Key]
        [Column("ID_EMPRESA")]
        public int Id { get; set; }

        [Required(ErrorMessage = "Campo nome é obrigatório")]
        [StringLength(120, ErrorMessage = "Campo não pode ter mais que 120 caracteres")]
        [Column("NM_EMPRESA")]
        public string Nome { get; set; } = string.Empty;

        [StringLength(300, ErrorMessage = "Campo não pode ter mais que 300 caracteres")]
        [Column("DS_EMPRESA")]
        public string? Descricao { get; set; }

        [StringLength(60, ErrorMessage = "Campo não pode ter mais que 60 caracteres")]
        [Column("DS_AREA")]
        public string? Area { get; set; }

        [Required(ErrorMessage = "Campo contratando agora é obrigatório")]
        [StringLength(1, ErrorMessage = "Campo não pode ter mais que 1 caractere")]
        [Column("FL_HIRING_NOW")]
        public string ContratandoAgora { get; set; } = "N";

        [StringLength(300, ErrorMessage = "Campo não pode ter mais que 300 caracteres")]
        [Column("DS_LOGO_URL")]
        public string? LogoUrl { get; set; }

        [StringLength(150, ErrorMessage = "Campo não pode ter mais que 150 caracteres")]
        [Column("DS_WEBSITE")]
        public string? Website { get; set; }

        [Column("DT_CRIACAO")]
        public DateTime DataCriacao { get; set; } = DateTime.Now;
    }
}