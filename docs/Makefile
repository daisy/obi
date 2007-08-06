# Make markdown documentation

XSLT =		xsltproc
INDENT =	xmllint --format
STYLESHEET =	template.xslt
HTML_FILES =	blocks.html events.html index.html \
		playback.html selection.html strip_manager_shortcuts.html \
		plan.html tasklist.html toolkit_audio.html operations.html

all:	$(HTML_FILES)

%.html:	%.txt
	echo "<markdown>" > $@.markdown && \
	markdown $< >> $@.markdown && \
	echo "</markdown>" >> $@.markdown && \
	$(XSLT) $(STYLESHEET) $@.markdown | $(INDENT) - > $@ && \
	$(RM) $@.markdown

.PHONY:	clean

clean:
	$(RM) *.markdown $(HTML_FILES)
