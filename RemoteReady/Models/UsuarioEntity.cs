using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RemoteReady.Models
{
    [Table("TB_GS_NET_USUARIO")]
    public class UsuarioEntity
    {
        [Key]
        [Column("ID_USUARIO")]
        public int Id { get; set; }

        [Required(ErrorMessage = "Campo nome é obrigatório")]
        [StringLength(100, ErrorMessage = "Campo não pode ter mais que 100 caracteres")]
        [Column("NM_USUARIO")]
        public string Nome { get; set; } = string.Empty;

        [Required(ErrorMessage = "Campo email é obrigatório")]
        [StringLength(120, ErrorMessage = "Campo não pode ter mais que 120 caracteres")]
        [Column("DS_EMAIL")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Campo senha é obrigatório")]
        [StringLength(60, ErrorMessage = "Campo não pode ter mais que 60 caracteres")]
        [Column("DS_PASSWORD")]
        public string Senha { get; set; } = string.Empty;

        [Required(ErrorMessage = "Campo tipo de usuário é obrigatório")]
        [StringLength(20, ErrorMessage = "Campo não pode ter mais que 20 caracteres")]
        [Column("TP_ROLE")]
        public string TipoUsuario { get; set; } = "USER";

        [Column("DT_CRIACAO")]
        public DateTime DataCriacao { get; set; } = DateTime.Now;
    }
}