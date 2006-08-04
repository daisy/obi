<?xml version="1.0" encoding="UTF-8"?>

<xsl:transform version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
  xmlns:rng="http://relaxng.org/ns/structure/1.0"
  xmlns:a="http://urakawa.sourceforge.net/ns-sandbox/annotations"
  exclude-result-prefixes="a rng" xmlns="http://www.w3.org/1999/xhtml">

  <xsl:output method="xml" version="1.0" encoding="UTF-8"/>

  <xsl:template match="/">
    <html xmlns="http://www.w3.org/1999/xhtml">
      <head>
        <meta http-equiv="Content-Type" content="text/xml+html; charset=UTF-8"/>
        <title>
        </title>
        <style type="text/css">
      body { background-color: white; color: black; font-family: sans-serif;
        margin: 2em; }
      ul { list-style-type: none; }
      .rng { background-color: #dfd; }
      .back { font-size: small; }
        </style>
      </head>
      <body>
        <xsl:apply-templates/>
      </body>
    </html>
  </xsl:template>

  <xsl:template match="rng:grammar">
    <h1>
      <xsl:value-of select="@a:title"/>
    </h1>
    <p>
      <xsl:text>All definitions: </xsl:text>
      <xsl:apply-templates select="rng:define" mode="list">
        <xsl:sort select="@name"/>
      </xsl:apply-templates>
      <xsl:text>.</xsl:text>
    </p>
    <xsl:apply-templates/>
  </xsl:template>

  <xsl:template match="rng:start">
    <h2 id="__start">start</h2>
    <ul>
      <xsl:apply-templates/>
    </ul>
  </xsl:template>

  <xsl:template match="rng:ref" mode="from">
    <xsl:choose>
      <xsl:when test="ancestor-or-self::rng:define">
        <a>
          <xsl:attribute name="href">
            <xsl:value-of select="concat('#',
              ancestor-or-self::rng:define[1]/@name)"/>
          </xsl:attribute>
          <xsl:value-of select="ancestor-or-self::rng:define[1]/@name"/>
        </a>
      </xsl:when>
      <xsl:otherwise>
        <a href="#__start">start</a>
      </xsl:otherwise>
    </xsl:choose>
    <xsl:choose>
      <xsl:when test="position()!=last()">
        <xsl:text>, </xsl:text>
      </xsl:when>
      <xsl:otherwise>
        <xsl:text>.</xsl:text>
      </xsl:otherwise>
    </xsl:choose>
  </xsl:template>

  <xsl:template match="rng:define">
    <h2 id="{@name}">
      <xsl:value-of select="@name"/>
    </h2>
    <ul>
      <xsl:apply-templates/>
      <li>
        <xsl:text>Referred to by: </xsl:text>
        <xsl:variable name="name">
          <xsl:value-of select="@name"/>
        </xsl:variable>
        <xsl:apply-templates select="//rng:ref[@name=$name]" mode="from"/>
      </li>
    </ul>
    <p class="back">
      <a href="#">[Back]</a>
    </p> 
  </xsl:template>

  <xsl:template match="rng:define" mode="list">
    <span>
      <a href="#{@name}">
        <xsl:value-of select="@name"/>
      </a>
      <xsl:if test="position()!=last()">
        <xsl:text>&#xa0;&#x2022; </xsl:text>
      </xsl:if>
    </span>
  </xsl:template>

  <xsl:template match="rng:ref">
    <li>
      <span class="rng">
        <xsl:value-of select="local-name()"/>
      </span>
      <xsl:text> </xsl:text>
      <a href="#{@name}">
        <xsl:value-of select="@name"/>
      </a>
      <xsl:if test="*">
        <ul>
          <xsl:apply-templates/>
        </ul>
      </xsl:if>
    </li>
  </xsl:template>

  <xsl:template match="rng:element|rng:attribute">
    <li>
      <span class="rng">
        <xsl:value-of select="local-name()"/>
      </span>
      <xsl:text> </xsl:text>
      <strong>
        <xsl:value-of select="@name"/>
      </strong>
      <xsl:text> [namespace: </xsl:text>
      <code>
        <xsl:apply-templates select="." mode="namespace"/>
      </code>
      <xsl:text>]</xsl:text>
      <xsl:if test="*">
        <ul>
          <xsl:apply-templates/>
        </ul>
      </xsl:if>
    </li>
  </xsl:template>

  <xsl:template match="rng:data">
    <li>
      <span class="rng">
        <xsl:value-of select="local-name()"/>
      </span>
      <xsl:text> </xsl:text>
      <strong>
        <xsl:value-of select="@type"/>
      </strong>
      <xsl:text> [datatype library: </xsl:text>
      <code>
        <xsl:apply-templates select="." mode="datatype-library"/>
      </code>
      <xsl:text>]</xsl:text>
    </li>
  </xsl:template>

  <xsl:template match="rng:*" mode="namespace">
    <xsl:choose>
      <xsl:when test="@ns">
        <xsl:value-of select="@ns"/>
      </xsl:when>
      <xsl:otherwise>
        <xsl:apply-templates select=".." mode="namespace"/>
      </xsl:otherwise>
    </xsl:choose>
  </xsl:template>

  <xsl:template match="rng:*" mode="datatype-library">
    <xsl:choose>
      <xsl:when test="@datatypeLibrary">
        <xsl:value-of select="@datatypeLibrary"/>
      </xsl:when>
      <xsl:otherwise>
        <xsl:apply-templates select=".." mode="datatype-library"/>
      </xsl:otherwise>
    </xsl:choose>
  </xsl:template>

  <xsl:template match="rng:value">
    <li>
      <span class="rng">
        <xsl:value-of select="local-name()"/>
      </span>
      <xsl:text> </xsl:text>
      <strong>
        <xsl:apply-templates/>
      </strong>
    </li>
  </xsl:template>

  <xsl:template match="rng:*">
    <li>
      <span class="rng">
        <xsl:value-of select="local-name()"/>
      </span>
      <xsl:if test="*">
        <ul>
          <xsl:apply-templates/>
        </ul>
      </xsl:if>
    </li>
  </xsl:template>

  <xsl:template match="rng:grammar/a:desc">
    <strong>Description: </strong>
    <xsl:apply-templates/>
  </xsl:template>

  <xsl:template match="a:desc">
    <li>
      <strong>Description: </strong>
      <xsl:apply-templates/>
    </li>
  </xsl:template>

  <xsl:template match="a:comment">
    <li>
      <strong>Comments: </strong>
      <xsl:apply-templates/>
    </li>
  </xsl:template>

  <xsl:template match="a:ref">
    <a href="#{@name}">
      <xsl:apply-templates/>
    </a>
  </xsl:template>

  <xsl:template match="a:elem">
    <xsl:text>&lt;</xsl:text>
    <xsl:apply-templates/>
    <xsl:text>&gt;</xsl:text>
  </xsl:template>

  <xsl:template match="a:em">
    <em>
      <xsl:apply-templates/>
    </em>
  </xsl:template>

  <xsl:template match="a:pre">
    <pre>
      <xsl:apply-templates/>
    </pre>
  </xsl:template>

  <xsl:template match="a:strong">
    <strong>
      <xsl:apply-templates/>
    </strong>
  </xsl:template>

</xsl:transform>
