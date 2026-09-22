# SVT 2.0 User Guide

Format: Confluence Wiki Markup.  
Images: `images/` folder next to this file (files with the `-en` suffix).  
Source (RU): `svt-2-user-guide.ru.confluence.md`

---

h1. SVT 2.0 User Guide

{info:title=Who this guide is for}
This document is intended for logistics and tariff specialists.

No IT background is required.

Reference-book field meanings are the same as in the current SVT system.
{info}

----

h2. 1. What SVT 2.0 is and how it differs from the legacy system

*SVT 2.0* is the new version of the Tariff Management System. At this stage you can *view* the reference books that have already been migrated.

||Action||Where to do it||
|View migrated reference books (rates, average rates, parities, transport legs, locations-nodes)|*SVT 2.0*|
|Load and change data for these reference books|*Legacy* SVT|
|View reference books that are *not* migrated yet|*Legacy* SVT|

{warning:title=Important}
SVT 2.0 is read-only. You cannot create, edit, or delete records here yet.
{warning}

SVT 2.0 data is synchronized from the legacy system. After a change in legacy SVT, the update usually appears in SVT 2.0 *within about 2 minutes*.

Exceptions:

* Every night from *01:00 to 02:30* updates wait until this window ends. On Saturdays there is an additional pause from *04:00 to 06:00*.
* A record deleted in the legacy system appears in SVT 2.0 after the nightly reconciliation, *around 02:00*.

----

h2. 2. How to open the system

Use one of the URLs below (corporate network access is required):

||Environment||URL||
|Test|[https://s001tst-as-svt.sibur.local/v2]|
|Prod|[https://s001as-svt.sibur.local/v2]|

# Open the link in the browser (Edge).
# If the browser prompts for credentials, sign in with your *Windows domain account* (the same one you use to log on to your PC).
# If the account *has a role* in SVT 2.0, the home page opens.
# If there is *no role*, the *Access denied* screen opens (section 9.1).

----

h2. 3. Menu and language

At the top of the page you will find the *Home* button, the domain menus, the current user, and the language selector.

!1-menu-en.png|width=800,alt="SVT 2.0 menu"!

*Home* opens the average-rates summary. The other reference books are grouped by domain: hover a domain name and choose a reference book. Items inside a domain are listed *alphabetically* by the label in the current language. A domain that has no reference books yet is not shown.

On the right side of the header are the *current user* (usually the display name; hover to see the login in the form *DOMAIN\account*) and the UI language (for example, *English (United States)*). When you change the language, button labels, menu items, and column labels change, and the order of reference books inside a domain is recalculated alphabetically. Column visibility settings are stored *separately for each language* (see section 7).

At the bottom of the page there is a footer bar: on the left, the load time of the current page; on the right, the application version and the build date/time. During the pilot this is needed so that a support request can name a specific build.

!2-footer-en.png|width=800,alt="Footer: load time and version"!

||Domain||Menu item||Page title (as shown in the system)||
|—|Home|Summary «Average rates by direction»|
|Rates|Average Rate|Average Rate|
|Rates|Parity rates|Parity rates|
|Rates|Rates|Rates|
|Routes|Locations-Nodes|Locations-Nodes|
|Routes|Transport Legs|Transport Legs|

----

h2. 4. Home page

The home page shows the *«Average rates by direction»* summary. This is a fixed slice; the user cannot configure it:

* product — polyolefins;
* rate type — agreement;
* transport kind — road (auto);
* no intermediate node;
* a six-month window: three months back and two months ahead of the current month;
* the set of directions is predefined.

!3-home-en.png|width=800,alt="Home: table and chart"!

* On the left — a table of directions and average rates by month.
* On the right — a bar chart for the *selected* row.
* To change the chart, click another row in the table.

The contents of this summary cannot be changed by the user.

----

h2. 5. Working with a reference book (common rules)

Hover a domain in the top menu and choose a reference book. A table of records opens.

*Common for all reference books:*

* Data is view-only.
* By default, *non-archive* (active) records are shown.
* To view archive records, enable the *Archive* column in Settings and filter by it (section 6.5).
* A filter row is under the column headers.
* You can sort: click a column header.
* Pagination is at the bottom (*«1 - 10 from … items»*). Use the arrows or page numbers.
* The arrow on the left of a row opens the *detail card* (all fields of the record).

Buttons to the right of the page title:

* *Reports* — Excel export (section 8).
* *Reset Filters* — clear all filters in the table.
* *Settings* — choose which columns to show (section 7).

h3. 5.1. Detail card

# Find the required row.
# Click the arrow on the left of the row.
# Field groups open (you can expand and collapse them). *Collapse all* / *Expand all* control all groups at once.

!4-averare-rate-en.png|width=800,alt="Average Rate detail card"!

----

h2. 6. Filters (detailed)

Filters are located *under the column headers*. You can set several filters at once — only rows that match *all* conditions remain.

To clear all filters, click *Reset Filters*.

The filter types below are shown for the *Average Rate* reference book. Other books use the same filter types; column names may differ.

h3. 6.1. Drop-down list

Used for fields such as *Rate type*, *Transport kind*, *Transport type*, *Currency*.

# In the filter row under the column, open the list (often labeled *Select*).
# Choose a value (for example, *Tender*).
# The table refreshes.

!5-average-rate-filter-dropdown-en.png|width=800,alt="Filter: rate type"!

h3. 6.2. Text search (nodes, names, product groups)

Used for *From*, *Proxy*, *To*, *Group*, *Product*, and similar text columns.

This is full-text search: the system finds records that contain the entered fragment.

# Click the filter field under the column.
# Enter part of the name (for example, *Kazan*, *port*, *China*).
# Wait for the table to refresh.

{tip:title=Tip}
Enter *at least 3 characters*. A shorter string will not work.
{tip}

You can combine several text filters (for example, From + Proxy + To).

!6-average-rate-filter-nodes-en.png|width=800,alt="Filter by nodes"!

h3. 6.3. Dates (start and end of the validity period)

Used for *Start*, *End*, and other date columns.

# Click the filter icon (funnel) on the date column — a list of operators opens.
# Choose an operator, for example:
** *Equals*
** *Greater Than* / *Greater Than Or Equals*
** *Less Than* / *Less Than Or Equals*
** *Clear* — remove the filter for this column
# Enter the date in the format shown in the field (typically *dd.mm.yyyy*) or pick it from the calendar.

!7-average-rate-filter-dates-en.png|width=800,alt="Date filter: operator selection"!

h3. 6.4. Numbers (rates, load, and similar)

Used for *Avg rate*, *Per ton*, *Load*, and other numeric columns.

# If needed, choose an operator next to the filter icon (*Equals*, *Greater Than*, *Less Than*, and so on).
# Enter a number (for example, *100*).
# The table shows matching rows. The footer shows how many records were found (for example, *«1 - 4 from 4 items»*).

!8-average-rate-filter-numbers-en.png|width=800,alt="Numeric filter by average rate"!

h3. 6.5. How to view archive records

By default, the archive is hidden.

# Click *Settings*.
# Enable the *Archive* column and click *Ok*.
# In the *Archive* column filter, select the required value (archive / active).
# When finished, you can hide the column again or click *Reset Filters*.

----

h2. 7. Configuring visible columns

If there are too many columns or a required column is missing, adjust visibility.

# Click *Settings*.
# Turn column switches on or off (green means the column is visible).
# *Ok* — save and close.
# *Cancel* — close without saving.
# *Reset* — restore the *default* column set (if settings were changed incorrectly).

!9-grid-settings-en.png|width=800,alt="Column settings dialog"!

{info:title=Where settings are stored}
Settings are stored in *your browser on this computer*, separately for *Russian* and *English*. On another computer or in another browser, the default column set is used again. Changing the language does not copy your settings from one language to the other.
{info}

{warning:title=Please note}
After *Ok* or *Reset* in column settings, the current table filters are usually cleared. Set the filters again if needed.
{warning}

----

h2. 8. Excel reports

Export respects the *current table filters*.

# Click *Reports*.
# Choose:
** *Short report* — only the columns turned on in *Settings*;
** *Full report* — an extended field set (as in the detail card).
# The Excel file is downloaded to your browser downloads folder.

!10-reports-menu-en.png|width=800,alt="Reports menu"!

If there are many rows, a confirmation dialog appears. For a short report it opens when there are more than *20 thousand* rows; for a full report, when there are more than *5 thousand*. The system warns about the row count and possible wait time. Click *Yes* to continue or *No* to cancel. After *Yes* the export continues: the dialog does not block the file size.

!11-reports-warning-en.png|width=800,alt="Large report confirmation"!

While the report is being generated, a status message may appear — wait until it finishes and do not close the tab.

{warning:title=Please note}
Prefer short reports when possible. They are generated about ~10 times faster than full reports.

The sizes below are a recommendation, not a system limit. Exports larger than these sizes are not blocked.

* Short reports are convenient to export up to *200 thousand* records.
* Full reports larger than *20 thousand* records are better narrowed with filters.
{warning}

----

h2. 9. No access, or the page did not open as expected

First look at *what is on the screen* — that decides what to do.

||What you see||What it is||What to do||
|*Access denied* card (no reference-book menu)|The account has no role in SVT 2.0|Section 9.1. Follow the *VKUS* link. This is not a program failure.|
|*Unable to verify access* card|The system could not verify permissions|Section 9.2. Wait and refresh the page.|
|Error banner about a program error|A failure while using a reference book|Section 9.3|
|Blank, spinning, or frozen page, no banner|A failure with no on-screen message|Section 9.4|

h3. 9.1. Access denied

This screen opens when domain sign-in succeeded, but the account has *no role* in SVT 2.0. There is no reference-book menu. The build version is still shown at the bottom.

The card shows the login (and the display name, if available) and the message in Russian and English. The *VKUS* link leads to information on how to get access. Resetting filters or refreshing the page is not needed here.

!12-acces-denied-en.png|width=600,alt="Access denied screen"!

h3. 9.2. Unable to verify access

This is *not the same* as access denied. The screen opens when the system *could not verify* permissions. Reference books do not open. There is no VKUS link on this screen.

Wait and open the URL again (or press F5). If the screen remains, submit a request (section 9.5).

!13-auth-unavaliable-en.png|width=600,alt="Unable to verify access screen"!

h3. 9.3. An error message is shown on the screen

If a reference book already opened but the page behaves oddly, try the following first:

* refresh the page (F5);
* click *Reset Filters*;
* in *Settings*, click *Reset* (default columns);
* open the same URL in another browser or in InPrivate / Incognito mode.

If that does not help, capture the materials below and submit a request (section 9.5).

Usually a banner appears at the top or in the page area:

_«A program error occurred. Click for details.»_

Do the following:

# Take a *screenshot of the whole page* (*PrtSc* / *Print Screen*, or *Win + Shift + S* to capture a region).
# Briefly write the *reproduction steps*, for example:
** opened Average Rate;
** entered «Kazan» in the From filter;
** clicked the row arrow — an error appeared.
# *Click* the error banner — details open (a dark box with text).
# Take a *second screenshot* with the details expanded, *or* select the text with the mouse (*Message*, *Type*, *StackTrace*), copy it (*Ctrl + C*), and paste it into the email / ticket.

!14-error-en.png|width=800,alt="Expanded error details"!

h3. 9.4. No error banner, but the page is blank, frozen, or the reference book did not load

First try the same steps as in section 9.3 (F5, reset filters, another browser). If that does not help, capture information from the *browser console*.

Step by step (Edge / Chrome):

# Do not close the tab with the problem.
# Press *F12* on the keyboard.
## If F12 does nothing, try *Ctrl + Shift + I*.
## On a laptop you may need *Fn + F12*.
# A developer panel opens on the right or at the bottom. This is normal: you need only one tab.
# At the top of the panel, find the *Console* tab (in a Russian UI it may be labeled *«Консоль»*). Click it.
# The console shows text lines (some may be red or yellow).
# Take a *screenshot* of the whole console window, *or* select the text with the mouse, copy it (*Ctrl + C*), and paste it into the request.
# Close the panel with the × button or press *F12* again.

!15-error-en.png|width=800,alt="Browser console (F12)"!

{tip:title=Do not interpret the text yourself}
Attaching a screenshot or the copied text is enough. You do not need to interpret the messages — support specialists will do that.
{tip}

h3. 9.5. Where to send the request

||Stage||Contact||
|Pilot industrial operation|email [vasilevivv@sibur.ru]|
|Production operation|ticket in *VKUS* (internal portal)|

In the request, include:

* environment (test or prod) and the page URL;
* what you did (step by step);
* what you expected and what happened;
* screenshots and the error text (for sections 9.3 and 9.4 — as described there).

----

h2. 10. Quick reference

||Task||What to do||
|Open a reference book|Domain menu in the header → reference book item|
|Find records|Filters under column headers|
|Clear search|*Reset Filters*|
|Show / hide columns|*Settings* → switches → *Ok*|
|Restore default columns|*Settings* → *Reset*|
|View all fields of a record|Arrow on the left of the row|
|Export to Excel|*Reports* → short or full|
|Change language|Language button on the right of the header|
|Data seems ~1 minute stale|Usually wait up to ~2 minutes after the change in legacy SVT. From 01:00 to 02:30 updates wait until the window ends (on Saturdays also 04:00–06:00). Deletions appear after reconciliation around 02:00|
