using System;
using AspNetCore.Identity.MongoDbCore.Models;
using MongoDbGenericRepository.Attributes;

namespace Test.Identity.Entities
{
    [CollectionName("Roles")]
    public class ApplicationRole:MongoIdentityRole<Guid>
    {

    }
}
