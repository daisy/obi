# Make markdown documentation

XSLT =		xsltproc
PERL =		perl
MARKDOWN =	markdown
INDENT =	xmllint --format
STYLESHEET =	template.xslt
HTML_FILES =	blocks.html events.html index.html manual.html plan.html\
		playback.html selection.html strip_manager_shortcuts.html \
		tasklist.html toolkit_audio.html operations.html features.html \
		recording.html keyboarding.html
TODO_HTML =	todo.html

all:	$(HTML_FILES) #todo

%.html:	%.txt
	echo "<markdown>" > $@.markdown && \
	$(MARKDOWN) $< >> $@.markdown && \
	echo "</markdown>" >> $@.markdown && \
	$(XSLT) $(STYLESHEET) $@.markdown | $(INDENT) - > $@ && \
	$(RM) $@.markdown

.PHONY:	clean todo

todo:	TODO
	$(PERL) todo.pl < $< > $(TODO_HTML)

clean:
	$(RM) *.markdown $(HTML_FILES) $(TODO_HTML)
