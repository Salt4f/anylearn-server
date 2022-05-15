using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AnyLearnServer.Models
{
    [Flags]
    public enum Categories
    {
        None =                      0,
        Idiomas =                   1 << 0,
        HabilidadesPersonales =     1 << 1,
        Tecnologia =                1 << 2,
        HistoriaAntropologia =      1 << 3,
        Sostenibilidad =            1 << 4,
        Comunicacion =              1 << 5,
        ArteDiseno =                1 << 6,
        CienciasMedicas =           1 << 7,
        Finanzas =                  1 << 8,
    }

    public class Course
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? Photo { get; set; }
        public int? Duration { get; set; }
        public Categories? Categories { get; set; }

    }

}
