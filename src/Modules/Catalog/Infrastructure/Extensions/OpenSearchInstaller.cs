using Catalog.Infrastructure.Model;

using OpenSearch.Client;
using OpenSearch.Net;

namespace Catalog.Infrastructure.Extensions;

internal static class OpenSearchProductIndex
{
    public const string Name = "products";
    public const string SearchIndex = "products_search";
}

public sealed class OpenSearchConfiguration
{
    public Uri Url { get; set; }
    public string UserName { get; set; }
    public string Password { get; set; }
}

public static class OpenSearchInstaller
{
    internal const string ProductsAnalyzer = "products_analyzer";
    internal const string ProductsQueryAnalyzer = "products_query_analyzer";
    internal const string ProductsNormaliser = "products_normaliser";
    public static async Task CreateIndexIfNotExists(IOpenSearchClient client)
    {
        var exists = await client.Indices.ExistsAsync(OpenSearchProductIndex.Name);
        if (exists.Exists)
        {
            return;
        }

        var response = await client.Indices.CreateAsync(OpenSearchProductIndex.Name, c => c
            .Settings(s => s
                .Analysis(a => a
                    .CharFilters(cf => cf
                        .PatternReplace("pattern_replace_char_filter", pr => pr
                            .Pattern("\\.")
                            .Replacement(" ")
                        )
                    )
                    .Tokenizers(t => t
                        .Standard("standard_tokenizer")
                    )
                    .TokenFilters(f => f
                        .Lowercase("lowercase_filter")
                        .Synonym("shops_synonyms", descriptor => descriptor.Synonyms("aleje", "aleja", "al"))
                        .EdgeNGram("edge_ngram_filter", e => e
                            .MinGram(1)
                            .MaxGram(25)
                        )
                    )
                    .Analyzers(an => an
                        .Custom(ProductsAnalyzer, ca => ca
                            .CharFilters("pattern_replace_char_filter", "mapping_char_filter")
                            .Tokenizer("standard_tokenizer")
                            .Filters("lowercase_filter", "shops_synonyms", "edge_ngram_filter")
                        )
                        .Custom(ProductsQueryAnalyzer, ca => ca
                            .CharFilters("pattern_replace_char_filter", "mapping_char_filter")
                            .Tokenizer("standard_tokenizer")
                            .Filters("lowercase_filter", "shops_synonyms")
                        )
                    )
                    .Normalizers(n => n.Custom(ProductsNormaliser,
                        cn => cn.CharFilters("pattern_replace_char_filter", "mapping_char_filter")
                            .Filters("lowercase_filter")))
                )
            )
            .Map<OpenSearchProduct>(m => m
                .AutoMap()
                .Properties(ps => ps
                    .Number(t => t
                        .Name(n => n.ProductId)
                        .Index(true)
                        .Store(true)
                    )
                    .Text(t => t
                        .Name(n => n.Name)
                        .Index(true)
                        .Store(true)
                        .CopyTo(f => f
                            .Field(OpenSearchProductIndex.SearchIndex))
                    )
                    .Number(t => t
                        .Name(n => n.PromotionalPrice)
                        .Index(true)
                        .Store(true)
                    )
                    .Number(b => b
                        .Name(n => n.Price)
                        .Index(true)
                        .Store(true)
                    )
                    .Text(b => b
                        .Name(n => n.Description)
                        .Index(true)
                        .Store(true)
                        .CopyTo(f => f
                            .Field(OpenSearchProductIndex.SearchIndex))
                    )
                    .Text(g => g
                        .Name(n => n.AvailableQuantity)
                        .Store(true)
                        .Index(true)
                    )
                    .Text(t => t
                        .Name(n => n.Tags)
                        .Index(true)
                        .Store(true)
                    )
                    .Date(t => t
                        .Name(n => n.DateCreated)
                        .Index(false)
                        .Store(true)
                    )
                    .Text(t => t
                        .Name(OpenSearchProductIndex.SearchIndex)
                        .Index(true)
                        .Store(false)
                        .SearchAnalyzer(ProductsQueryAnalyzer) // used for searching
                        .Analyzer(ProductsAnalyzer)
                    )
                )
            )
        );

        if (!response.IsValid)
        {
            throw new InvalidOperationException($"can't create index, {response.DebugInformation}");
        }
    }

    public static IOpenSearchClient Setup(OpenSearchConfiguration configuration)
    {
        var settings = new ConnectionSettings(configuration.Url)
            .ServerCertificateValidationCallback(CertificateValidations.AllowAll)
            .DefaultIndex(OpenSearchProductIndex.Name)
            .DefaultMappingFor<OpenSearchProduct>(m => m
                .IndexName(OpenSearchProductIndex.Name)
                .IdProperty(x => x.ProductId)
            );
        if (configuration is { UserName: not null, Password: not null})
        {
            settings.BasicAuthentication(configuration.UserName, configuration.Password);
        }

        return new OpenSearchClient(settings);
    }
}