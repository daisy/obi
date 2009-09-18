<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
  xmlns:xuk="http://www.daisy.org/urakawa/xuk/1.0"
  xmlns:obi="http://www.daisy.org/urakawa/obi">

  <xsl:output method="xml"/>
  <xsl:variable name="text-channel" select="//xuk:mChannelItem[xuk:Channel[@name='obi.text']]/@uid"/>

  <!-- Filter out unused content -->
  <xsl:template match="obi:*[@used='False']"/>
  
  <!-- Filter out used sections without any phrase children, or with no used phrase children.
  Issue a warning if there were any. -->
  <xsl:template match="obi:section[not(xuk:mChildren/obi:phrase[not(@used='False')])]">
    <xsl:message terminate="no">
      <xsl:text>Section "</xsl:text>
      <xsl:value-of select=".//xuk:mChannelMapping[@channel=$text-channel]/xuk:TextMedia/xuk:mText"/>
      <xsl:text>" has no text content; it will not be exported.</xsl:text>
    </xsl:message>
  </xsl:template>
  
  <xsl:template match="*">
    <xsl:copy>
      <xsl:copy-of select="@*"/>
      <xsl:apply-templates/>
    </xsl:copy>
  </xsl:template>
  
</xsl:stylesheet>