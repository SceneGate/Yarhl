using Markdig.Extensions.Emoji;
using Statiq.Markdown;

return await Bootstrapper
  .Factory
  .CreateDocs(args)
  .AddSolutionFiles("../src/Yarhl.sln")
  .ModifyTemplate(
    MediaTypes.Markdown,
    x => ((RenderMarkdown)x)
      .UseExtension(new EmojiExtension(EmojiMapping.DefaultEmojisOnlyMapping)))
  .RunAsync();
