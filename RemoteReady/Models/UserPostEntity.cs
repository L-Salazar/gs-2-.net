using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RemoteReady.Models
{
    [Table("TB_GS_NET_USER_POST")]
    public class UserPostEntity
    {
        [Key]
        [Column("ID_USER_POST")]
        public int Id { get; set; }

        [Required(ErrorMessage = "Campo ID do usuário é obrigatório")]
        [Column("ID_USUARIO")]
        public int IdUsuario { get; set; }

        [Required(ErrorMessage = "Campo ID do post é obrigatório")]
        [Column("ID_POST")]
        public int IdPost { get; set; }

        [Required(ErrorMessage = "Campo status é obrigatório")]
        [StringLength(20, ErrorMessage = "Campo não pode ter mais que 20 caracteres")]
        [Column("DS_STATUS")]
        public string Status { get; set; } = "LIDO";

        [Required(ErrorMessage = "Campo data de leitura é obrigatório")]
        [Column("DT_LEITURA")]
        public DateTime DataLeitura { get; set; } = DateTime.Now;

        // Relacionamentos (navegação)
        [ForeignKey("IdUsuario")]
        public virtual UsuarioEntity? Usuario { get; set; }

        [ForeignKey("IdPost")]
        public virtual BlogPostEntity? Post { get; set; }
    }
}