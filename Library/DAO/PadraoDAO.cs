using Library.VO;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.DAO
{
    abstract class PadraoDAO
    {
        protected string Tabela { get; set; }
        protected string Chave { get; set; } = "id"; // valor default
        protected abstract string MontaSQLInsert();
        protected abstract string MontaSQLUpdate();
        protected abstract SqlParameter[] CriaParametros(PadraoVO o);
        protected abstract PadraoVO MontaVO(DataRow dr);
        protected virtual string MontaSQLDelete()
        {
            return $"delete {Tabela} where {Chave} = @id";
        }
        protected virtual string MontaSQLConsulta()
        {
            return $"select * from {Tabela} where {Chave} = @id";
        }
        
        public virtual void Inserir(PadraoVO o)
        {
            if (Consulta(o.Id) != null)
                throw new Exception("Este código já está sendo utilizado!");
            string sql = MontaSQLInsert();
            MetodosBD.ExecutaSQL(sql, CriaParametros(o));
        }
       
        public virtual void Alterar(PadraoVO o)
        {
            string sql = MontaSQLUpdate();
            MetodosBD.ExecutaSQL(sql, CriaParametros(o));
        }


        public virtual void Excluir(int Id)
        {
            string sql = MontaSQLDelete();
            SqlParameter[] parametros = new SqlParameter[1];
            parametros[0] = new SqlParameter(Chave, Id);
            MetodosBD.ExecutaSQL(sql, parametros);
        }
       
        public PadraoVO Consulta(int id)
        {
            using (SqlConnection cx = ConexaoBD.GetConexao())
            {
                string sql = MontaSQLConsulta();
                SqlParameter[] parametros =
                {
                    new SqlParameter(Chave, id)
                };
                DataTable tabela = MetodosBD.ExecutaSelect(sql, parametros);
                if (tabela.Rows.Count == 0)
                    return null;
                else
                {
                    return MontaVO(tabela.Rows[0]);
                }
            }
        }
       
        public virtual int ProximoId()
        {
            string sql = $"select isnull(max({Chave})+1,1) from {Tabela}";
            using (SqlConnection cx = ConexaoBD.GetConexao())
            {
                SqlCommand cmd = new SqlCommand(sql, cx);
                return Convert.ToInt32(cmd.ExecuteScalar());
            }
        }
        
        public virtual PadraoVO Primeiro()
        {
            string sql = $"select top 1 * from {Tabela} order by {Chave}";
            return ExecutaSqlLocal(sql, null);
        }


        public virtual PadraoVO Ultimo()
        {
            string sql = $"select top 1 * from {Tabela} order by {Chave} desc";
            DataTable tabela = MetodosBD.ExecutaSelect(sql, null);
            return ExecutaSqlLocal(sql, null);
        }
        
        public virtual PadraoVO Proximo(int atual)
        {
            string sql = $"select top 1 * from {Tabela} where {Chave} > @Atual order by {Chave} ";
            SqlParameter[] p =
            {
                new SqlParameter("Atual", atual)
            };
            return ExecutaSqlLocal(sql, p);
        }
        
        public virtual PadraoVO Anterior(int atual)
        {
            string sql = $"select top 1 * from {Tabela} where {Chave} < @Atual order by {Chave} desc";
            SqlParameter[] p =
            {
                new SqlParameter("Atual", atual)
            };
            return ExecutaSqlLocal(sql, p);
        }
       
        protected PadraoVO ExecutaSqlLocal(string sql, SqlParameter[] parametros)
        {
            DataTable tabela = MetodosBD.ExecutaSelect(sql, parametros);
            if (tabela.Rows.Count == 0)
                return null;
            else
                return MontaVO(tabela.Rows[0]);
        }
    }
}
