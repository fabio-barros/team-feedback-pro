using FluentAssertions;
using System.ComponentModel;
using TeamFeedbackPro.Domain.Enums;

namespace TeamFeedbackPro.Domain.Tests.Enums;

public class EnumHelperTests
{
    [Theory]
    [InlineData(FeedbackType.Positive, "Positivo")]
    [InlineData(FeedbackType.Constructive, "Construtivo")]
    [InlineData(FeedbackType.Critical, "Crítico")]
    public void ToDescription_FeedbackType_ReturnsExpectedDescription(FeedbackType value, string expected)
    {
        value.ToDescription().Should().Be(expected);
    }

    [Theory]
    [InlineData(FeedbackCategory.Teamwork, "Trabalho em equipe")]
    [InlineData(FeedbackCategory.CodeQuality, "Qualidade de código")]
    [InlineData(FeedbackCategory.Communication, "Comunicação")]
    [InlineData(FeedbackCategory.ProblemSolving, "Resolução de problema")]
    [InlineData(FeedbackCategory.Leadership, "Liderança")]
    [InlineData(FeedbackCategory.Other, "Outro")]
    public void ToDescription_FeedbackCategory_ReturnsExpectedDescription(FeedbackCategory value, string expected)
    {
        value.ToDescription().Should().Be(expected);
    }

    [Theory]
    [InlineData(FeedbackStatus.Pending, "Em análise")]
    [InlineData(FeedbackStatus.Approved, "Aprovado")]
    [InlineData(FeedbackStatus.Rejected, "Rejeitado")]
    public void ToDescription_FeedbackStatus_ReturnsExpectedDescription(FeedbackStatus value, string expected)
    {
        value.ToDescription().Should().Be(expected);
    }

    public enum NoDescriptionEnum
    {
        ValueWithoutDescription
    }

    [Fact]
    public void ToDescription_WithoutDescriptionAttribute_ReturnsEnumName()
    {
        const NoDescriptionEnum value = NoDescriptionEnum.ValueWithoutDescription;
        value.ToDescription().Should().Be("ValueWithoutDescription");
    }

    [Fact]
    public void GetAttribute_ReturnsDescriptionAttribute()
    {
        var attr = FeedbackType.Positive.GetAttribute<DescriptionAttribute>();
        attr.Should().NotBeNull();
        attr!.Description.Should().Be("Positivo");
    }

    [Fact]
    public void GetAttribute_WithoutAttribute_ReturnsNull()
    {
        const NoDescriptionEnum value = NoDescriptionEnum.ValueWithoutDescription;
        value.GetAttribute<DescriptionAttribute>().Should().BeNull();
    }
}