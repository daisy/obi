<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet version="1.0"
  xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
  xmlns:smil="http://www.w3.org/2001/SMIL20/" 
  xmlns:xuk="http://www.daisy.org/urakawa/xuk/1.0"
  xmlns:obi="http://www.daisy.org/urakawa/obi"
  xmlns:ncx="http://www.daisy.org/z3986/2005/ncx/"
  xmlns:ext="http://www.daisy.org/urakawa/obi/xslt-extensions"
  exclude-result-prefixes="xuk obi"
  extension-element-prefixes="ext">
  <xsl:output method="xml" indent="yes"/>

  <!-- Total time of the book in milliseconds -->
  <xsl:param name="total-time"/>

  <!-- Name of the UID element -->
  <xsl:param name="uid">UID</xsl:param>

  <!-- IDs of the various channels -->
  <xsl:variable name="text-channel" select="//xuk:mChannelItem[xuk:Channel[@name='obi.text']]/@uid"/>
  <xsl:variable name="publish-channel" select="//xuk:mChannelItem[xuk:Channel[@name='obi.publish.audio']]/@uid"/>

  <xsl:template match="/">
    <!-- wrapper for the whole fileset -->
    <z>

      <!-- the package file -->
      <package xmlns="http://openebook.org/namespaces/oeb-package/1.0/"
        unique-identifier="{$uid}">
        <metadata>
          <dc-metadata>
            <xsl:for-each select="//xuk:Metadata[starts-with(@name,'dc:')]">
              <xsl:element name="{substring-after(@name,'dc:')}" namespace="http://purl.org/dc/elements/1.1/">
                <xsl:if test="@name='dc:Identifier'">
                  <xsl:attribute name="id">
                    <xsl:value-of select="$uid"/>
                  </xsl:attribute>
                  <xsl:attribute name="scheme">DTB</xsl:attribute>
                </xsl:if>
                <xsl:value-of select="@content"/>
              </xsl:element>
            </xsl:for-each>
            <Format xmlns="http://purl.org/dc/elements/1.1/">ANSI/NISO Z39.86-2005</Format>
            <Type xmlns="http://purl.org/dc/elements/1.1/">Sound</Type>
          </dc-metadata>
          <x-metadata>
            <xsl:for-each select="//xuk:Metadata[not(starts-with(@name,'dc:'))]">
              <meta name="{@name}" content="{@content}"/>
            </xsl:for-each>
            <meta name="dtb:multimediaType" content="audioNCX"/>
            <meta name="dtb:multimediaContent" content="audio"/>
            <meta name="dtb:totalTime" content="{ext:TotalElapsedTime(count(//obi:section))}ms"/>
            <meta name="dtb:audioFormat" content="WAV"/>
          </x-metadata>
        </metadata>
        <manifest>
          <item href="{ext:RelativePath('.opf')}" id="OPF"  media-type="text/xml"/>
          <item href="{ext:RelativePath('.ncx')}" id="NCX" media-type="application/x-dtbncx+xml"/>
          <xsl:for-each select="//obi:section">
            <item href="{ext:RelativePath('.smil', generate-id())}" id="{generate-id()}"
              media-type="application/smil"/>
          </xsl:for-each>
          <xsl:for-each select="//xuk:ExternalAudioMedia">
            <xsl:variable name="src" select="@src"/>
            <xsl:if test="not(preceding::xuk:ExternalAudioMedia[@src=$src])">
              <item href="{ext:RelativePathForUri($src)}" id="{generate-id()}" media-type="audio/x-wav"/>
            </xsl:if>
          </xsl:for-each>
        </manifest>
        <spine>
          <xsl:for-each select="//obi:section">
            <itemref idref="{generate-id()}"/>
          </xsl:for-each>
        </spine>
      </package>

      <!-- The NCX -->
      <ncx:ncx version="2005-1" xml:lang="{//xuk:Metadata[@name='dc:Language']/@content}">
        <ncx:head>
          <ncx:meta name="dtb:uid" content="{//xuk:Metadata[@name='dc:Identifier']/@content}"/>
          <xsl:for-each select="//obi:section">
            <xsl:sort order="descending" select="count(ancestor-or-self::obi:section)"/>
            <xsl:if test="position()=1">
              <ncx:meta name="dtb:depth" content="{count(ancestor-or-self::obi:section)}"/>
            </xsl:if>
          </xsl:for-each>
          <ncx:meta name="dtb:generator" content="{//xuk:Metadata[@name='dtb:generator']/@content}"/>
          <ncx:meta name="dtb:totalPageCount" content="{count(//obi:phrase[@kind='Page'])}"/>
          <xsl:choose>
            <xsl:when test="//obi:phrase[@kind='Page']">
              <xsl:for-each select="//obi:phrase[@kind='Page']">
                <xsl:sort order="descending" select="@page"/>
                <xsl:if test="position()=1">
                  <ncx:meta name="dtb:maxPageNumber" content="{@page}"/>
                </xsl:if>
              </xsl:for-each>
            </xsl:when>
            <xsl:otherwise>
              <ncx:meta name="dtb:maxPageNumber" content="0"/>  
            </xsl:otherwise>
          </xsl:choose>
        </ncx:head>
        <!-- TODO: title/author role in Obi -->
        <ncx:docTitle>
          <ncx:text>
            <xsl:value-of select="//xuk:Metadata[@name='dc:Title']/@content"/>
          </ncx:text>
        </ncx:docTitle>
        <ncx:navMap>
          <xsl:apply-templates mode="navPoint"/>        
        </ncx:navMap>
        <ncx:pageList>
          <xsl:apply-templates select="//obi:phrase[@kind='Page']"/>      
        </ncx:pageList>
      </ncx:ncx>

      <!-- The smil files; one per section -->
      <xsl:for-each select="//obi:section">
        <smil:smil id="{generate-id()}">
          <smil:head>
            <smil:meta name="dtb:uid" content="{//xuk:Metadata[@name='dc:Identifier']/@content}"/>
            <smil:meta name="dtb:generator" content="{//xuk:Metadata[@name='dtb:generator']/@content}"/>
            <smil:meta name="dtb:totalElapsedTime" content="{ext:TotalElapsedTime(position()-1)}ms"/>
            <!-- Keep track of which section this SMIL file is for -->
            <smil:meta name="obi:section" content="{.//xuk:mChannelMapping[@channel=$text-channel]/xuk:TextMedia/xuk:mText}"/>
          </smil:head>
          <smil:body>
            <smil:seq id="{generate-id(xuk:mChildren)}">
              <xsl:for-each select="xuk:mChildren/obi:phrase//xuk:mChannelMapping[@channel=$publish-channel]/xuk:ExternalAudioMedia">
                <smil:par id="{generate-id()}">
                  <smil:audio src="{ext:RelativePathForUri(@src)}" clipBegin="{@clipBegin}" clipEnd="{@clipEnd}"/>
                </smil:par>
              </xsl:for-each>
            </smil:seq>
          </smil:body>
        </smil:smil>
      </xsl:for-each>

    </z>
  </xsl:template>

  <!-- Navigation point for section -->
  <xsl:template match="obi:section" mode="navPoint">
    <ncx:navPoint id="{generate-id()}" 
      playOrder="{1+count(ancestor::obi:section|preceding::obi:section|ancestor::obi:phrase[@kind='Page']|preceding::obi:phrase[@kind='Page'])}">
      <ncx:navLabel>
        <ncx:text>
          <!-- The section node has text media -->
          <xsl:value-of select=".//xuk:mChannelMapping[@channel=$text-channel]/xuk:TextMedia/xuk:mText"/>
        </ncx:text>
        <!-- The audio heading is the phrase with the "heading" role or the first one -->
        <xsl:choose>
          <xsl:when test="xuk:mChildren/obi:phrase[@kind='Heading']">
            <xsl:variable name="h" select="xuk:mChildren/obi:phrase[@kind='Heading']//xuk:mChannelMapping[@channel=$publish-channel]/xuk:ExternalAudioMedia"/>
            <ncx:audio src="{ext:RelativePathForUri($h/@src)}" clipBegin="{$h/@clipBegin}" clipEnd="{$h/@clipEnd}"/>
          </xsl:when>
          <xsl:otherwise>
            <xsl:variable name="p" select="xuk:mChildren/obi:phrase[1]//xuk:mChannelMapping[@channel=$publish-channel]/xuk:ExternalAudioMedia"/>
            <ncx:audio src="{ext:RelativePathForUri($p/@src)}" clipBegin="{$p/@clipBegin}" clipEnd="{$p/@clipEnd}"/>
          </xsl:otherwise>
        </xsl:choose>
      </ncx:navLabel>
      <ncx:content
        src="{ext:RelativePath('.smil', generate-id())}#{generate-id(xuk:mChildren/obi:phrase[1]//xuk:mChannelMapping[@channel=$publish-channel]/xuk:ExternalAudioMedia)}"/>
      <xsl:apply-templates mode="navPoint"/>
    </ncx:navPoint>
  </xsl:template>

  <!-- Page target -->
  <xsl:template match="obi:phrase[@kind='Page']">
    <ncx:pageTarget value="{@page}" type="normal"
      playOrder="{1+count(ancestor::obi:section|preceding::obi:section|ancestor::obi:phrase[@kind='Page']|preceding::obi:phrase[@kind='Page'])}">
      <ncx:navLabel>
        <ncx:text>
          <xsl:value-of select="@page"/>
        </ncx:text>
        <xsl:variable name="p" select=".//xuk:mChannelMapping[@channel=$publish-channel]/xuk:ExternalAudioMedia"/>
        <ncx:audio src="{ext:RelativePathForUri($p/@src)}" clipBegin="{$p/@clipBegin}" clipEnd="{$p/@clipEnd}"/>
      </ncx:navLabel>
      <ncx:content src="{ext:RelativePath('.smil', generate-id(ancestor::obi:section[1]))}#{generate-id(.//xuk:mChannelMapping[@channel=$publish-channel]/xuk:ExternalAudioMedia)}"/>
    </ncx:pageTarget>
  </xsl:template>

  <xsl:template match="text()"/>
  <xsl:template match="text()" mode="navPoint"/>
  
</xsl:stylesheet>