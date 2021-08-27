using System;
using AspNetCore.Identity.MongoDbCore.Models;
using MongoDbGenericRepository.Attributes;

namespace Test.Identity.Entities
{

    [CollectionName("Users")]
    public class ApplicationUser: MongoIdentityUser<Guid>
    {


            public decimal Gil { get; set; }



        
    }
}
