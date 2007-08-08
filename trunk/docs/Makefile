# Make markdown documentation

XSLT =		xsltproc
INDENT =	xmllint --format
STYLESHEET =	template.xslt
HTML_FILES =	blocks.html events.html index.html manual.html plan.html\
		playback.html selection.html strip_manager_shortcuts.html \
		tasklist.html toolkit_audio.html operations.html features.html

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
