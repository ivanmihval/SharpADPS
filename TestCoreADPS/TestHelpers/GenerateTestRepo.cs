using System;
using System.Collections.Generic;
using System.IO;
using CoreADPS;
using CoreADPS.MailModels;
using CoreADPS.Storage;
using CoreADPS.Storage.Models;
using WPFSharpADPS.Helpers.HashsumEngine;

namespace TestCoreADPS.TestHelpers
{
    public class GenerateTestRepo
    {
        public static List<Mail> Generate(string repoFolderPath)
        {
            var originalAtachmentsFolder = Path.Combine(repoFolderPath, "originals");
            var messagesFolder = Path.Combine(repoFolderPath, Storage.MessagesFolder);
            var attachmentsFolder = Path.Combine(repoFolderPath, Storage.AttachmentsFolder);

            Directory.CreateDirectory(originalAtachmentsFolder);
            Directory.CreateDirectory(messagesFolder);
            Directory.CreateDirectory(attachmentsFolder);

            var attachment1Content = Unicode.Utf8WithouBom.GetBytes("12345");
            var attachment2Content = Unicode.Utf8WithouBom.GetBytes("12345677899");
            var attachment3Content = Unicode.Utf8WithouBom.GetBytes("12345");
            var attachment4Content = Unicode.Utf8WithouBom.GetBytes("123123123123");

            var attachment1Path = Path.Combine(originalAtachmentsFolder, "test.txt");
            var attachment2Path = Path.Combine(originalAtachmentsFolder, "document");
            var attachment3Path = Path.Combine(originalAtachmentsFolder, "document.txt");
            var attachment4Path = Path.Combine(originalAtachmentsFolder, "document.bin");

            File.WriteAllBytes(attachment1Path, attachment1Content);
            File.WriteAllBytes(attachment2Path, attachment2Content);
            File.WriteAllBytes(attachment3Path, attachment3Content);
            File.WriteAllBytes(attachment4Path, attachment4Content);

            var attachment1Info = MailAttachmentInfo.FromFilePath(attachment1Path);
            var attachment2Info = MailAttachmentInfo.FromFilePath(attachment2Path);
            var attachment3Info = MailAttachmentInfo.FromFilePath(attachment3Path);
            var attachment4Info = MailAttachmentInfo.FromFilePath(attachment4Path);

            var mail1 = new Mail(
                dateCreated: new DateTime(2020, 1, 1),
                recipientsCoordinates: new List<Coordinates> { new Coordinates(55.0, 37.0) },
                name: "Donald Smith",
                additionalNotes: null,
                inlineMessage: "Please see 2 attachments",
                attachments: new List<Attachment>
                    {
                        Attachment.FromMailAttachmentInfo(attachment1Info),
                        Attachment.FromMailAttachmentInfo(attachment2Info),
                        Attachment.FromMailAttachmentInfo(attachment3Info),
                    }
                );

            var mail2 = new Mail(
                dateCreated: new DateTime(2019, 3, 4),
                recipientsCoordinates: new List<Coordinates> { new Coordinates(54.0, 36.0) },
                name: "abcde@abcde.com",
                additionalNotes: null,
                inlineMessage: "The document is in attachment",
                attachments: new List<Attachment>
                    {
                        Attachment.FromMailAttachmentInfo(attachment3Info),
                        Attachment.FromMailAttachmentInfo(attachment4Info),
                    }
                );

            var storage = new Storage(repoFolderPath);
            storage.SaveMail(mail1, new List<MailAttachmentInfo> {attachment1Info, attachment2Info, attachment3Info},
                             new DotNetHashsumEngine().Calculate);
            storage.SaveMail(mail2, new List<MailAttachmentInfo> {attachment3Info, attachment4Info},
                             new DotNetHashsumEngine().Calculate);

            return new List<Mail> {mail1, mail2};
        }
    }
}
