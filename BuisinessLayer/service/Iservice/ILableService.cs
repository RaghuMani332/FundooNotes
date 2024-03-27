using CommonLayer.Models.RequestDto;
using CommonLayer.Models.ResponceDto;
using RepositaryLayer.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuisinessLayer.service.Iservice
{
    public interface ILableService
    {
        LableResponce CreateLable(LableRequest request);
        String? DeleteLable(string lableName, string userEmail);
        LableResponce GetByLableId(int lableId);
        List<LableResponce> GetLableByEmail(string userEmail);
        LableResponce UpdateLable(LableRequest request);
    }
}
