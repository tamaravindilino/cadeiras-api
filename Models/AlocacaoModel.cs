using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace CadeirasAPI.Models
{
    public class AlocacaoModel
    {
        public Guid id { get; set; }

        // FK para a tabela Cadeira
        [ForeignKey("Cadeira")]
        public Guid cadeira_id { get; set; }

        public DateTime data_hora_init { get; set; }
        public DateTime data_hora_fim { get; set; }

        public AlocacaoModel() { }

        public AlocacaoModel(Guid cadeiraId, DateTime inicio, DateTime fim)
        {
            id = Guid.NewGuid();
            cadeira_id = cadeiraId;
            data_hora_init = inicio;
            data_hora_fim = fim;
        }

        public virtual CadeiraModel? Cadeira { get; set; }
    }
}