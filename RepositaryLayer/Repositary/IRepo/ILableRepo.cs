using RepositaryLayer.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepositaryLayer.Repositary.IRepo
{
    public interface ILableRepo
    {
        LableEntity CreateLable(LableEntity entity,bool flag);
        int DeleteLable(string lableName, int userId);
        public LableEntity getLabelById(int Id);
        List<LableEntity> GetLableByEmail(int userId);
        LableEntity UpdateLable(LableEntity lableEntity,bool flag);
    }
}
