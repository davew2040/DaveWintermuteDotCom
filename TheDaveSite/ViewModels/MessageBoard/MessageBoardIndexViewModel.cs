using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DataAccess.Models;

namespace TheDaveSite.ViewModels.MessageBoard
{
    public class MessageBoardIndexViewModel
    {
        private IEnumerable<DataAccess.Models.MessageBoard> _messageBoards = null;
        public IEnumerable<DataAccess.Models.MessageBoard> MessageBoards
        {
            get
            {
                if (null == _messageBoards)
                {
                    _messageBoards = new List<DataAccess.Models.MessageBoard>();
                }
                return _messageBoards;
            }
            set
            {
                _messageBoards = value;
            }
        }
    }
}