<?xml version="1.0" encoding="UTF-8"?>

<!-- Template for markdown documentation -->
<!-- $Id$ -->

<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">

  <xsl:output method="xml" encoding="UTF-8"
    doctype-public="-//W3C//DTD XHTML 1.0 Strict//EN"
    doctype-system="http://www.w3.org/TR/xhtml1/DTD/xhtml1-strict.dtd"/>

  <xsl:template match="/markdown">
    <html xmlns="http://www.w3.org/1999/xhtml" xml:lang="en" lang="en">
      <head>
        <title>
          <xsl:value-of select="//h1"/>
        </title>
        <meta http-equiv="Content-Type" content="text/html; charset=UTF-8"/>
        <link rel="stylesheet" type="text/css" href="obi.css"/>
      </head>
      <body>
        <xsl:apply-templates/>
      </body>
    </html>
  </xsl:template>

  <!-- Prettify h1 -->
  <xsl:template match="h1">
    <xsl:element name="div">
      <xsl:attribute name="id">top</xsl:attribute>
      <xsl:element name="{name()}">
        <xsl:copy-of select="@*"/>
        <xsl:apply-templates/>
      </xsl:element>
    </xsl:element>
  </xsl:template>

  <!-- Add a table of contents -->
  <xsl:template match="div[@id='toc']">
    <xsl:element name="{name()}">
      <xsl:copy-of select="@*"/>
      <xsl:element name="h2">
        <xsl:apply-templates/>
      </xsl:element>
      <xsl:element name="ul">
        <xsl:apply-templates select="following::h2" mode="toc"/>
      </xsl:element>
    </xsl:element>
  </xsl:template>

  <xsl:template match="*" mode="toc-a">
    <xsl:element name="a">
      <xsl:attribute name="href">
        <xsl:choose>
          <xsl:when test="@id">
            <xsl:value-of select="concat('#',@id)"/>
          </xsl:when>
          <xsl:otherwise>
            <xsl:value-of select="concat('#',generate-id())"/>
          </xsl:otherwise>
        </xsl:choose>
      </xsl:attribute>
      <xsl:apply-templates/>
    </xsl:element>
  </xsl:template>

  <xsl:template match="h2" mode="toc">
    <xsl:element name="li">
      <xsl:apply-templates select="." mode="toc-a"/>
      <xsl:if test="following::*[self::h2 or self::h3][1][self::h3]">
        <xsl:element name="ul">
          <xsl:apply-templates select="following::h3[generate-id(preceding::h2[1])=generate-id(current())]" mode="toc"/>
        </xsl:element>
      </xsl:if>
    </xsl:element>
  </xsl:template>

  <xsl:template match="h3" mode="toc">
    <xsl:element name="li">
      <xsl:apply-templates select="." mode="toc-a"/>
      <xsl:if test="following::*[self::h3 or self::h4][1][self::h4]">
        <xsl:element name="ul">
          <xsl:apply-templates select="following::h4[generate-id(preceding::h3[1])=generate-id(current())]" mode="toc"/>
        </xsl:element>
      </xsl:if>
    </xsl:element>
  </xsl:template>

  <xsl:template match="h4" mode="toc">
    <xsl:element name="li">
      <xsl:apply-templates select="." mode="toc-a"/>
    </xsl:element>
  </xsl:template>

  <!-- Add ids to headings that we link to if they don't have any -->
  <xsl:template match="h2[not(@id)]|h3[not(@id)]|h4[not(@id)]">
    <xsl:element name="{name()}">
      <xsl:copy-of select="@*"/>
      <xsl:attribute name="id">
        <xsl:value-of select="generate-id()"/>
      </xsl:attribute>
      <xsl:apply-templates/>
    </xsl:element>
  </xsl:template>

  <!-- Link to source files -->
  <xsl:template match="code[text()[contains(.,'.cs')]]">
    <xsl:element name="a">
      <xsl:attribute name="href">
        <xsl:value-of select="concat('../Obi/Obi/',.)"/>
      </xsl:attribute>
      <xsl:element name="{name()}">
        <xsl:copy-of select="@*"/>
        <xsl:apply-templates/>
      </xsl:element>
    </xsl:element>
  </xsl:template>

  <!-- Indent paragaphs following other paragraphs -->
  <xsl:template match="p[preceding-sibling::*[1][name()='p']]">
    <xsl:element name="{name()}">
      <xsl:copy-of select="@*"/>
      <xsl:attribute name="class">indent</xsl:attribute>
      <xsl:apply-templates/>
    </xsl:element>
  </xsl:template>

  <!-- Highlight TODO in red. -->
  <xsl:template match="text()[contains(.,'TODO')]">
    <xsl:value-of select="substring-before(.,'TODO')"/>
    <xsl:element name="span">
      <xsl:attribute name="class">todo</xsl:attribute>
      <xsl:text>TODO</xsl:text>
    </xsl:element>
    <xsl:value-of select="substring-after(.,'TODO')"/>
  </xsl:template>

  <!-- Dump the rest as is -->
  <xsl:template match="*">
    <xsl:element name="{name()}">
      <xsl:copy-of select="@*"/>
      <xsl:apply-templates/>
    </xsl:element>
  </xsl:template>

</xsl:stylesheet>
