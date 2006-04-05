<?xml version="1.0" encoding="UTF-8"?>

<!-- Transform the a annotations in an XHTML document -->
<!-- $Id$ -->

<xsl:transform version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
  xmlns:html="http://www.w3.org/1999/xhtml"
  xmlns:a="http://pom.clacbec.net/xmlns/a"
  xmlns:date="http://exslt.org/dates-and-times"
  xmlns="http://www.w3.org/1999/xhtml"
  extension-element-prefixes="date" exclude-result-prefixes="html a">

  <xsl:output method="xml" version="1.0" encoding="UTF-8"
    doctype-public="-//W3C//DTD XHTML 1.1//EN"
    doctype-system="http://www.w3.org/TR/xhtml11/DTD/xhtml11.dtd"/>

  <!-- Format an e-mail address with a link -->
  <xsl:template match="a:email">
    <code>
      <xsl:text>&lt;</xsl:text>
      <a href="mailto:{@addr}">
        <xsl:value-of select="@addr"/>
      </a>
      <xsl:text>&gt;</xsl:text>
    </code>
  </xsl:template>

  <!-- Make a quick table of contents (inline) -->
  <xsl:template match="a:quick-toc">
    <xsl:if test="//a:section">
      <span id="{generate-id(.)}">
        <xsl:text> Quick TOC: </xsl:text>
        <xsl:for-each select="//a:section/html:h2">
          <a href="#{../@id}">
            <xsl:apply-templates/>
          </a>
          <xsl:choose>
            <xsl:when test="position()=last()">
              <xsl:text>.</xsl:text>
            </xsl:when>
            <xsl:otherwise>
              <xsl:text>&#xa0;&#x2022; </xsl:text>
            </xsl:otherwise>
          </xsl:choose>
        </xsl:for-each>
      </span>
    </xsl:if>
  </xsl:template>

  <!-- Make a reference to a section -->
  <xsl:template match="a:ref">
    <a href="#{@label}">
      <xsl:text>section </xsl:text>
      <xsl:apply-templates select="//a:section[@id=current()/@label]"
        mode="number"/>
    </a>
  </xsl:template>

  <!-- Make a section with a link back to the TOC (for top-level sections only,
  not subsections.) -->
  <xsl:template match="a:section">
    <div>
      <xsl:copy-of select="@id"/>
      <xsl:apply-templates/>
      <xsl:if test="//a:quick-toc and not(parent::a:section)">
        <p class="back">
          <a href="#{generate-id(//a:quick-toc)}">
            <xsl:text>[TOC]</xsl:text>
          </a>
        </p>
      </xsl:if>
    </div>
  </xsl:template>

  <xsl:template match="a:section" mode="number">
    <xsl:number count="a:section" level="multiple"/>
  </xsl:template>

  <!-- Insert today's date (using exslt functions) -->
  <xsl:template match="a:today">
    <xsl:choose>
      <xsl:when test="function-available('date:date')">
        <xsl:value-of select="date:date()"/>
      </xsl:when>
      <xsl:otherwise>
        <xsl:text>today (?)</xsl:text>
      </xsl:otherwise>
    </xsl:choose>
  </xsl:template>

  <!-- Number the section headings -->
  <xsl:template match="html:h2|html:h3">
    <xsl:element name="{local-name()}">
      <xsl:copy-of select="@*"/>
      <xsl:number count="a:section" level="multiple"/>
      <xsl:text> </xsl:text>
      <xsl:apply-templates/>
    </xsl:element>
  </xsl:template>

  <!-- Copy any XHTML elements as is -->
  <xsl:template match="html:*">
    <xsl:element name="{local-name()}">
      <xsl:copy-of select="@*"/>
      <xsl:apply-templates/>
    </xsl:element>
  </xsl:template>

</xsl:transform>
