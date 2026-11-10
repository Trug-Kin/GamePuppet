using System;
using System.Collections.Generic;
using System.Linq;

namespace ConRoi.Investigation.V36.R107
{
    // GDD5.2 exact working plaintext. Pose keys are technical IDs shared by all consumers;
    // no mesh angle or final glyph is invented here. Punctuation is not a coded symbol.
    public static class FullInvestigationLetters
    {
        public const string Alphabet="ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        public static string[] ReplyPhrases=>new[]{"STAND BY","AGREED","STATION","NINE AT NIGHT"};
        public static string[] Ids=>new[]{"letter.bao","letter","letter.mai","letter.tung","letter.lan"};
        public static string Recipient(string id)=>id=="letter.bao"?"Bảo":id=="letter"?"Hà":id=="letter.mai"?"Mai":id=="letter.tung"?"Tùng":id=="letter.lan"?"Lan":"";
        internal static string Plain(string id)
        {
            switch(id){
                case "letter.bao": return "STAND BY WHO KEEPS THE STORE KEY ASK LAN KEEP QUIET";
                case "letter": return "STAND BY WHERE DID THE THREE CRATES GO SEND WORD";
                case "letter.mai": return "STAND BY FIND THE OLD STORE NOTE SEND WORD";
                case "letter.tung": return "STAND BY DO YOU STILL SEE LAN SEND WORD";
                case "letter.lan": return "STAND BY BRING THE OLD PUPPETS TO THE STATION NINE AT NIGHT OR THE OLD SALE COMES OUT";
                default: return null;
            }
        }
        // Public encoding exposes pose IDs, not a plaintext preview. Bind them to the same
        // 26-glyph asset catalog for letter, Henri lookup and puppet.
        public static string[] PoseSequence(string id)
        {
            var plain=Plain(id);return plain==null?Array.Empty<string>():plain.Select(c=>c==' '?"word-break":"pose."+((int)(c-'A')+1).ToString("D2")).ToArray();
        }
        public static string PoseKey(char letter)=>letter>='A'&&letter<='Z'?"pose."+((int)(letter-'A')+1).ToString("D2"):"";
        internal static string Read(string id,IEnumerable<CharacterAssignment> mapping)
        {
            var plain=Plain(id);if(plain==null)return "";
            return string.Concat(plain.Select(c=>c==' '?" • ":mapping.FirstOrDefault(m=>m.pose==PoseKey(c))?.letter is string value&&value.Length==1?value:"_"));
        }
        public static string Glossary=>"STAND BY: chờ lệnh; WHO: ai; KEEPS: giữ; STORE KEY: chìa kho; ASK LAN: hỏi Lan; KEEP QUIET: giữ kín; WHERE DID: đã ở đâu; THREE CRATES: ba thùng; GO: đi; SEND WORD: gửi lời lại; FIND: tìm; OLD STORE NOTE: ghi chú kho cũ; DO YOU STILL SEE LAN: còn gặp Lan không; BRING: mang; OLD PUPPETS: rối cũ; STATION: ga; NINE AT NIGHT: chín giờ tối; OR THE OLD SALE COMES OUT: nếu không chuyện bán rối bị phơi ra; AGREED: đồng ý.";
    }
}
