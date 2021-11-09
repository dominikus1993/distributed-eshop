using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Chat.Core.Model;
using Chat.Core.Services;
using Xunit;

namespace Chat.Core.UnitTests.Services;

public class DefaultMessageOrderProviderTests
{

    [Fact]
    public async Task GetMessageOrder_Returns_Correct_Order()
    {
        // Arrange
        var id1 = Guid.NewGuid();
        var id2 = Guid.NewGuid();
        var id3 = Guid.NewGuid();
        var messages = new List<ChatMessage>
        {
            new ChatMessage (id1, "janpawel3", "xDD",  DateTime.Now.AddDays(-2) ),
            new ChatMessage (id2, "papaj", "xDD",  DateTime.Now.AddDays(-3) ),
            new ChatMessage (id3, "karolwojtylak", "xDD",  DateTime.Now.AddDays(-1) ),
        };

        var provider = new DefaultMessageOrderProvider();
        var subject = await provider.Sort(messages).ToListAsync();
        Assert.Equal(3, subject.Count);
        Assert.Equal(id3, subject[0].Id);
    }
}
