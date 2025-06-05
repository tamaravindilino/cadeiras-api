namespace CadeirasAPI.Models;

public class CadeiraModel
{
    public Guid id { get; set; }
    public int numero { get; set; }
    public string descricao { get; set; } = string.Empty;

    public CadeiraModel() { }

    public CadeiraModel(int num, string descricao_)
    {
        if (num <= 0)
            throw new ArgumentException("Número inválido", nameof(num));

        id = Guid.NewGuid();
        numero = num;
        descricao = descricao_;
    }

    public void Change(int num, string descricao_)
    {
        // Valida o numero 
        if (num <= 0)
            throw new ArgumentException("Número inválido", nameof(num));

        // Valida a descricao 
        if (string.IsNullOrWhiteSpace(descricao))
            throw new ArgumentException("Descrição inválida", nameof(descricao));

        numero = num;
        descricao = descricao_;
    }
}