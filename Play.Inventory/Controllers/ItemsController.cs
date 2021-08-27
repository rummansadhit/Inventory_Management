using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Play.Common;
using Play.Inventory.Service;
using Play.Inventory.Service.Dtos;
using Play.Inventory.Service.Entities;


namespace Play.Inventory.Controllers
{

    [ApiController]
    [Route("items")]
    public class ItemsController : ControllerBase
    {

        private IRepository<InventoryItem> inventoryItemsRepository;
        private IRepository<CatalogItem> catalogItemsRepository;


        private const string AdminRole = "Admin";

        public ItemsController(IRepository<InventoryItem> InventoryItemRepository, IRepository<CatalogItem> CatalogItemRepository)
        {
            inventoryItemsRepository = InventoryItemRepository;

            catalogItemsRepository = CatalogItemRepository;
        }


        [HttpGet]
        public async Task<ActionResult<IEnumerable<InventoryItemDto>>> GetAsync(Guid userId)
        {
            if (userId == Guid.Empty)
            {
                return BadRequest();
            }

            //var currentUserId = User.FindFirstValue("sub");

           // if (Guid.Parse(currentUserId) != userId)
           // {
           //     if (!User.IsInRole(AdminRole))
          //      {
          //          return Unauthorized();
          //      }
          //  }

            var inventoryItemEntities = await inventoryItemsRepository.GetAllAsync(item => item.UserId == userId);
            var itemIds = inventoryItemEntities.Select(item => item.CatalogItemId);
            var catalogItemEntities = await catalogItemsRepository.GetAllAsync(item => itemIds.Contains(item.Id));

            var inventoryItemDtos = inventoryItemEntities.Select(inventoryItem =>
            {
                var catalogItem = catalogItemEntities.Single(catalogItem => catalogItem.Id == inventoryItem.CatalogItemId);
                return inventoryItem.AsDto(catalogItem.Name, catalogItem.Description);
            });

            return Ok(inventoryItemDtos);
        }


        [HttpPost]
        //[Authorize(Roles = AdminRole)]
        public async Task<ActionResult> PostAsync(GrantItemsDto grantItemsDto)
        {
            var inventoryItem = await inventoryItemsRepository.GetAsync(
                item => item.UserId == grantItemsDto.UserId && item.CatalogItemId == grantItemsDto.CatalogItemId);

            if (inventoryItem == null)
            {
                inventoryItem = new InventoryItem
                {
                    CatalogItemId = grantItemsDto.CatalogItemId,
                    UserId = grantItemsDto.UserId,
                    Quantity = grantItemsDto.Quantity,
                    AcquiredData = DateTimeOffset.UtcNow
                };

                await inventoryItemsRepository.CreateAsync(inventoryItem);
            }
            else
            {
                inventoryItem.Quantity += grantItemsDto.Quantity;
                await inventoryItemsRepository.UpdateAsync(inventoryItem);
            }

            return Ok();
        }




    }
}
