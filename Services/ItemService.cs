// MIT License
// Copyright DNN Community

using Dnn.Modules.SecurityCenter.Common.Extensions;
using Dnn.Modules.SecurityCenter.Data.Entities;
using Dnn.Modules.SecurityCenter.Data.Repositories;
using Dnn.Modules.SecurityCenter.DTO;
using Dnn.Modules.SecurityCenter.Services;
using Dnn.Modules.SecurityCenter.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dnn.Modules.SecurityCenter.Services
{
    /// <summary>
    /// Provides services to manage items.
    /// </summary>
    public class ItemService : IItemService
    {
        private IRepository<Item> itemRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="ItemService"/> class.
        /// </summary>
        /// <param name="itemRepository">The items repository.</param>
        public ItemService(IRepository<Item> itemRepository)
        {
            this.itemRepository = itemRepository;
        }

        /// <inheritdoc/>
        /// <exception cref="ArgumentNullException"> is thrown if the item or one of its required properties are missing.</exception>
        public async Task<ItemViewModel> CreateItemAsync(CreateItemDTO item, int userId)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            if (string.IsNullOrWhiteSpace(item.Name))
            {
                throw new ArgumentNullException("The item name is required.", nameof(item.Name));
            }

            var newItem = new Item() { Name = item.Name, Description = item.Description };
            await this.itemRepository.CreateAsync(newItem, userId);

            return new ItemViewModel(newItem);
        }

        /// <inheritdoc/>
        public async Task<ItemsPageViewModel> GetItemsPageAsync(string query, int page = 1, int pageSize = 10, bool descending = false)
        {
            var items = await this.itemRepository.GetPageAsync(
                page,
                pageSize,
                entities => entities
                    .Where(item => string.IsNullOrEmpty(query) || item.Name.ToUpper().Contains(query.ToUpper()))
                    .Order(item => item.Name, descending));

            var itemsPageViewModel = new ItemsPageViewModel()
            {
                Items = items.Items.Select(item => new ItemViewModel
                {
                    Description = item.Description,
                    Id = item.Id,
                    Name = item.Name,
                }).ToList(),
                Page = items.Page,
                ResultCount = items.ResultCount,
                PageCount = items.PageCount,
            };

            return itemsPageViewModel;
        }

        /// <inheritdoc/>
        public async Task DeleteItemAsync(int itemId)
        {
            await this.itemRepository.DeleteAsync(itemId);
        }

        /// <inheritdoc/>
        public async Task UpdateItemAsync(UpdateItemDTO dto, int userId)
        {
            if (dto is null)
            {
                throw new ArgumentNullException(nameof(dto));
            }

            if (string.IsNullOrWhiteSpace(dto.Name))
            {
                throw new ArgumentNullException(nameof(dto.Name));
            }

            var item = await this.itemRepository.GetByIdAsync(dto.Id);
            item.Name = dto.Name;
            item.Description = dto.Description;

            await this.itemRepository.UpdateAsync(item, userId);
        }
    }
}
