using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs;
using API.Entities;
using API.Helpers;
using API.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class MessageRepository : IMessageRepository
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;
        public MessageRepository(DataContext context, IMapper mapper)
        {
            _mapper = mapper;
            _context = context;
        }

        public void AddMessage(Message message)
            => _context.Messages.Add(message);

        public void DeleteMessage(Message message)
            => _context.Messages.Remove(message);

        public async Task<Message> GetMessage(int id)
            => await _context.Messages
                .Include(m => m.Sender)
                .Include(m => m.Recipient)
                .SingleOrDefaultAsync(x => x.Id == id);

        public async Task<PagedList<MessageDto>> GetMessagesForUser(MessageParams messageParams)
        {
            var query = _context.Messages.OrderByDescending(m => m.MessageSent).AsQueryable();
            query = messageParams.Container switch
            {
                "Inbox" => query.Where(u => u.Recipient.UserName.Equals(messageParams.Username) && !u.RecipientDeleted),
                "Outbox" => query.Where(u => u.Sender.UserName.Equals(messageParams.Username) && !u.SenderDeleted),
                _ => query.Where(u => u.Recipient.UserName.Equals(messageParams.Username) && !u.RecipientDeleted && u.DateRead == null)
            };
            var messages = query.ProjectTo<MessageDto>(_mapper.ConfigurationProvider);

            return await PagedList<MessageDto>.CreateAsync(messages, messageParams.PageNumber, messageParams.PageSize);
        }

        public async Task<IEnumerable<MessageDto>> GetMessageThread(string currentUsername, string recipientUsername)
        {
            var messages = await _context.Messages
                .Include(m => m.Sender).ThenInclude(u => u.Photos)
                .Include(m => m.Recipient).ThenInclude(u => u.Photos)
                .Where(m => (m.Recipient.UserName.Equals(currentUsername) && !m.RecipientDeleted && m.Sender.UserName.Equals(recipientUsername))
                    || (m.Recipient.UserName.Equals(recipientUsername) && !m.SenderDeleted && m.Sender.UserName.Equals(currentUsername)))
                .OrderBy(m => m.MessageSent)
                .ToListAsync();

            // Ungelesene Nachrichten als gelesen markieren.
            var unreadMessages = messages.Where(m => m.DateRead == null && m.Recipient.UserName.Equals(currentUsername)).ToList();
            if (unreadMessages.Any())
            {
                foreach (var message in unreadMessages)
                {
                    message.DateRead = DateTime.Now;
                }

                await _context.SaveChangesAsync();
            }

            return _mapper.Map<IEnumerable<MessageDto>>(messages);
        }

        public async Task<bool> SaveAllAsync()
            => await _context.SaveChangesAsync() > 0;
    }
}