using BihuApiCore.EntityFrameworkCore.Models;
using BihuApiCore.Repository.IRepository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace BihuApiCore.Repository.Repositories
{
    public class ProductRepository : EfRepositoryBase<Product>, IProductRepository
    {
        public ProductRepository(DbContext context) : base(context)
        {
        }
    }
}
