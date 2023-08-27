return await Bootstrapper
  .Factory
  .CreateDocs(args)
  .AddSolutionFiles("../src/Yarhl.sln")
  .RunAsync();
