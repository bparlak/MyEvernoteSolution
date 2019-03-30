using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyEvernote.Entities
{
    public class MyEntityBase
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [DisplayName("Oluşturulduğu Tarih"), Required, ScaffoldColumn(false)]
        public DateTime CreatedOn { get; set; }

        [DisplayName("Son Değiştirilme Tarihi"), Required, ScaffoldColumn(false)]
        public DateTime ModifiedOn { get; set; }

        [DisplayName("Oluşturan Kişi"), Required, StringLength(30), ScaffoldColumn(false)]
        public string ModifiedUsername { get; set; }
    }
}
